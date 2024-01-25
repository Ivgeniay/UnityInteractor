using UnityEngine;

namespace NodeEngine.Database.Error
{
    internal class DSErrorData
    {
        internal Color Color { get; set; }
        internal DSErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte)Random.Range(65, 256),
                (byte)Random.Range(50, 176),
                (byte)Random.Range(50, 176),
                255
            );
        }
    }
}
