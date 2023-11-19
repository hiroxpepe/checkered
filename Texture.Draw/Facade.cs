﻿// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the GPL v2.0 license. See LICENSE text in the project root for license information.

using System;
using System.Drawing;
using System.Linq;

using Texture.Core;
using Texture.Draw;
using static Texture.Core.Cut;
using static Texture.Core.Face;
using static Texture.Core.Swing;
using static Texture.Draw.Palette;
using static Texture.Draw.Switch;
using static Texture.Draw.Tool;

namespace Texture {
    /// <summary>
    /// facade class
    /// </summary>
    /// <company>STUDIO MeowToon</company>
    /// <author>Hiroyuki Adachi</author>
    public static class Facade {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random _random = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb, verb phrases]

        public static void Execute() {
            // output size.
            Rectangle rect = new(x: 0, y: 0, width: 256, height: 256);
            // for layer 1.
            Cut cut1 = NewCutByPiece(piece_count_x: 7, piece_count_y: 7);
            Face face1 = NewFace(width: rect.Width, height: rect.Height, cut: cut1, crop: 0.9d);
            Palette palette1 = NewPalette(
                primary: Color.Green, 
                secondary: Color.Yellow,
                accent: Color.Lime,
                alpha: 1.0f
            );
            Swing swing1 = NewSwing(min_value_x: 10, max_value_x: 10, min_value_y: 10, max_value_y: 10);
            // for layer 2.
            Cut cut2 = NewCutByPiece(piece_count_x: 6, piece_count_y: 6);
            Face face2 = NewFace(width: rect.Width, height: rect.Height, cut: cut2, crop: 0.9d);
            Palette palette2 = NewPalette(
                primary: Color.Lime,
                secondary: Color.Azure,
                alpha: 0.5f
            );
            Swing swing2 = NewSwing(min_value_x: 20, max_value_x: 20, min_value_y: 20, max_value_y: 20);
            // creates tool
            using Tool tool = NewTool(rect: rect, face_array: new Face[] { face1, face2 });

            // callback for layer 1.
            int count = 0; int img_idx = 0;
            face1.OnReady += () => {
                // randomize
                face1.AllPoint.ForEach(action: x => {
                    x.Where(predicate: x => !x.FixedX).ToList().ForEach(action: x => {
                        if (!x.MovedX) { 
                            x.X += _random.Next(minValue: swing1.MinValueX, maxValue: swing1.MaxValueX);
                            x.MovedX = true;
                        }
                    });
                    x.Where(predicate: x => !x.FixedY).ToList().ForEach(action: x => {
                        if (!x.MovedY) {
                            x.Y += _random.Next(minValue: swing1.MinValueY, maxValue: swing1.MaxValueY);
                            x.MovedY = true;
                        }
                    });
                });
                // fills by color.
                InitIndex(count_x: face1.CountX, count_y: face1.CountY);
                face1.AllPoint.ForEach(action: x => {
                    Color color = NextIndex() % 2 == 0 ? palette1.Primary : palette1.Secondary;
                    tool.Fill(points: x, color: color, img_idx: img_idx, cell_idx: count, debug: false);
                    int random_number = _random.Next(1, (face1.AllPoint.Count / 2) + 1);
                    if (random_number == 1) {
                        tool.Fill(points: x, color: palette1.Accent, img_idx: img_idx, cell_idx: count);
                    }
                    count++;
                });
            };
            face1.OnWrite += () => {
                tool.Write(img_idx, alpha: palette1.Alpha, angle: 0);
            };
            // execute layer 1.
            face1.Make();

            // callback for layer 2.
            count = 0; img_idx++;
            face2.OnReady += () => {
                // randomize
                face2.AllPoint.ForEach(action: x => {
                    x.Where(predicate: x => !x.FixedX).ToList().ForEach(action: x => {
                        if (!x.MovedX) {
                            x.X += _random.Next(minValue: swing2.MinValueX, maxValue: swing2.MaxValueX);
                            x.MovedX = true;
                        }
                    });
                    x.Where(predicate: x => !x.FixedY).ToList().ForEach(action: x => {
                        if (!x.MovedY) {
                            x.Y += _random.Next(minValue: swing2.MinValueY, maxValue: swing2.MaxValueY);
                            x.MovedY = true;
                        }
                    });
                });
                // fills by color.
                InitIndex(count_x: face2.CountX, count_y: face2.CountY);
                face2.AllPoint.ForEach(action: x => {
                    Color color = NextIndex() % 2 == 0 ? palette2.Primary : palette2.Secondary;
                    tool.Fill(points: x, color: color, img_idx: img_idx, cell_idx: count);
                    count++;
                });
            };
            face2.OnWrite += () => {
                tool.Write(img_idx, alpha: palette2.Alpha, angle: 0);
            };
            // execute layer 2.
            face2.Make();
        }
    }
}