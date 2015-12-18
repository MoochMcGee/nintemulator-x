namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        private void Blend()
        {
            Layer[] layers = new Layer[5]
            {
                bg0,
                bg1,
                bg2,
                bg3,
                spr
            };

            for (uint x = 0; x < 256; x++)
            {
                uint color1 = cram.h[0];
                uint color2 = fixedcolor;
                uint priority1 = 0;
                uint priority2 = 0;
                uint source1 = 5;
                uint source2 = 5;

                for (uint i = 0; i < 5; i++)
                {
                    var layer = layers[i];

                    if (layer.enable[x])
                    {
                        if (layer.sm != 0 && priority1 < layer.priority[x]) { priority1 = layer.priority[x]; source1 = i; }
                        if (layer.ss != 0 && priority2 < layer.priority[x]) { priority2 = layer.priority[x]; source2 = i; }
                    }
                }

                bool colorexempt = (source1 == 4 && (color1 < 0xc0));

                if (source1 != 5) color1 = cram.h[layers[source1].raster[x]];
                if (source2 != 5) color2 = cram.h[layers[source2].raster[x]];

                if (!colorexempt && math_enable[source1])
                {
                    uint r1 = (color1 >> 0) & 31, g1 = (color1 >> 5) & 31, b1 = (color1 >> 10) & 31;
                    uint r2 = (color2 >> 0) & 31, g2 = (color2 >> 5) & 31, b2 = (color2 >> 10) & 31;

                    r1 += r2;
                    g1 += g2;
                    b1 += b2;

                    if (r1 > 31) r1 = 31;
                    if (g1 > 31) g1 = 31;
                    if (b1 > 31) b1 = 31;

                    color1 = (r1 << 0) | (g1 << 5) | (b1 << 10);
                }

                raster[x] = colors[color1];
            }
        }

        private void RenderMode0()
        {
            bg0.Render(Bg.BPP2);
            bg1.Render(Bg.BPP2);
            bg2.Render(Bg.BPP2);
            bg3.Render(Bg.BPP2);
            spr.Render();

            Blend();
        }
        private void RenderMode1()
        {
            bg0.Render(Bg.BPP4);
            bg1.Render(Bg.BPP4);
            bg2.Render(Bg.BPP2);
            spr.Render();

            Blend();
        }
        private void RenderMode2() { /* Offset-per-tile */ }
        private void RenderMode3()
        {
            bg0.Render(Bg.BPP8);
            bg1.Render(Bg.BPP4);
            spr.Render();

            for (uint i = 0; i < 256; i++)
            {
                uint color = 0;

                if ((color = spr.GetColorM(i, 3)) != 0) goto render; // Sprites with priority 3
                if ((color = bg0.GetColorM(i, 1)) != 0) goto render; // BG1 tiles with priority 1
                if ((color = spr.GetColorM(i, 2)) != 0) goto render; // Sprites with priority 2
                if ((color = bg1.GetColorM(i, 1)) != 0) goto render; // BG2 tiles with priority 1
                if ((color = spr.GetColorM(i, 1)) != 0) goto render; // Sprites with priority 1
                if ((color = bg0.GetColorM(i, 0)) != 0) goto render; // BG1 tiles with priority 0
                if ((color = spr.GetColorM(i, 0)) != 0) goto render; // Sprites with priority 0
                if ((color = bg1.GetColorM(i, 0)) != 0) goto render; // BG2 tiles with priority 0

            render:
                raster[i] = colors[cram.h[color]];
            }
        }
        private void RenderMode4() { /* Offset-per-tile */ }
        private void RenderMode5() { /* Hi-res */ }
        private void RenderMode6() { /* Hi-res */ }
        private void RenderMode7()
        {
            bg0.RenderAffine();
            spr.Render();

            Blend();
        }
    }
}