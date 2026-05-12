public static class Crc32
{
    static readonly uint[] Table = CreateTable();

    /// <summary>
    /// Creates the CRC32 lookup table using the polynomial 0xEDB88320. 
    /// This table is used to efficiently compute the CRC32 hash for byte arrays.
    /// </summary>
    /// <returns></returns>
    static uint[] CreateTable()
    {
        uint poly = 0xEDB88320u;
        var table = new uint[256];

        for (uint i = 0; i < table.Length; i++)
        {
            uint crc = i;
            for (int j = 0; j < 8; j++)
                crc = (crc & 1) != 0 ? (crc >> 1) ^ poly : crc >> 1;

            table[i] = crc;
        }

        return table;
    }

    /// <summary>
    /// Computes the CRC32 hash for the given byte array using the precomputed lookup table.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static uint Compute(byte[] data)
    {
        uint crc = 0xFFFFFFFFu;

        for (int i = 0; i < data.Length; i++)
            crc = (crc >> 8) ^ Table[(crc ^ data[i]) & 0xFF];

        return ~crc;
    }
}
