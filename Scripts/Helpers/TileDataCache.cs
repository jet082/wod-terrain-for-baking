using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;

namespace Monobelisk
{
    public class TileDataCache
    {
        private readonly Dictionary<string, byte[]> tileDataCache = new Dictionary<string, byte[]>();

        /// <summary>
        /// Get tileData for a map pixel during TerrainTexturer invokation.
        /// </summary>
        /// <param name="mapPixelX"></param>
        /// <param name="mapPixelY"></param>
        /// <returns></returns>
        public byte[] Get(int mapPixelX, int mapPixelY)
        {
            var pos = PositionKey(mapPixelX, mapPixelY);

            if (tileDataCache.ContainsKey(pos))
            {
                var td = tileDataCache[pos];
                tileDataCache.Remove(pos);

                return td;
            }

            Debug.LogWarning("==> Interesting Terrains: No tileData found for map pixel " + mapPixelX + "x" + mapPixelY);

            return null;
        }

        /// <summary>
        /// Store tileData for a map pixel for use in TerrainTexturer.
        /// Uses indexer assignment so a re-promotion of the same tile (which
        /// DFU's StreamingWorld can issue after save-load or floating-origin
        /// shifts) silently overwrites the stale entry instead of throwing
        /// ArgumentException: An item with the same key has already been added.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tileData"></param>
        public void Add(DFPosition pos, byte[] tileData)
        {
            tileDataCache[PositionKey(pos.X, pos.Y)] = tileData;
        }

        /// <summary>
        /// Empty the cache. Useful for offline tools (e.g. distance-field
        /// bakers) that drive ScheduleGenerateSamplesJob across the whole
        /// world map without going through the normal TerrainTexturer drain.
        /// </summary>
        public void Clear()
        {
            tileDataCache.Clear();
        }

        /// <summary>
        /// Remove cached tileData, if no TerrainTexturer has used it, to avoid memory buildup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="terrainData"></param>
        public void UncacheTileData(DaggerfallTerrain sender, TerrainData terrainData)
        {
            var pos = PositionKey(sender.MapPixelX, sender.MapPixelY);
            if (tileDataCache.ContainsKey(pos))
            {
                tileDataCache.Remove(pos);
            }
        }

        private static string PositionKey(int mapPixelX, int mapPixelY)
        {
            return new DFPosition(mapPixelX, mapPixelY).ToString().Trim();
        }
    }
}