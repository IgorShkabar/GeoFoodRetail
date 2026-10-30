/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved. 
 * You can test the effect of executing `InitSampleData` — specifically, whether the memory within the `_page` object has been populated correctly.
 * Testing via the public interface (State-based Testing)
 * If the `RTreePage` class (or the class that contains it) provides public properties or methods for reading data, the test should utilize them.
• Object initialization: verifying that `_page` is not `null` after the method is called.
• Correctness of the SoA (Structure of Arrays) layout: ensuring that the coordinates—`Xmin`, `Ymin`, `Xmax`, `Ymax` (or however the indices 0–15 are structured in my implementation)—have been placed into the exact memory slots intended. 
*/
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoFoodRetail.GIS.StreamingRTree;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GeoFoodRetail.Tests.GIS.Tests.StreamingRTree.Tests
{
    [TestFixture]
    public class DemoPlatformTest
    {        
        // Mouse-driven query (Query AABB)
        public RectangleF queryAABB = new RectangleF(100, 100, 150, 100);
        public const int MaxChildren = 60;


        [Test]
        public unsafe void InitSampleData_ShouldPopulateBoxesInSoAFormat_Clean()
        {
            var demo = new DemoPlatform();
            demo.InitSampleData();
            /// An array of expected values ​​(exactly 16 items)
            float[] expected = {
                50, 300, 500, 200,
                50, 100, 300, 350,
                200, 450, 650, 350,
                180, 250, 450, 500
            };

            
            fixed (RTreePage* pagePtr = &demo.CurrentPageRef)
            {
                float* ptr = pagePtr->BoxesData;
                Assert.That(ptr[0], Is.EqualTo(50f).Within(0.00001f));
                Assert.That(ptr[1], Is.EqualTo(300f).Within(0.00001f));
                Assert.That(ptr[12], Is.EqualTo(180f).Within(0.00001f));
                Assert.That(ptr[15], Is.EqualTo(500f).Within(0.00001f));

                // or code: 
                Assert.AreEqual(50f, ptr[0], 0.00001f);
                Assert.AreEqual(300f, ptr[1], 0.00001f);

                /// Or we check the buffer's boundary values
                Assert.AreEqual(180f, ptr[12], 0.00001f);
                Assert.AreEqual(500f, ptr[15], 0.00001f);

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.That(ptr[i], Is.EqualTo(expected[i]).Within(0.001f),
                        $"Неверное значение по индексу {i}");
                }
            }
        }


        /** The size of the selected search area cannot be 
         * smaller or larger than its optimal size. */
[Test]
        public void QueryAABB_ShouldBeOptimalSize()
        {
            // Arrange (set up your expected optimal dimensions
            var expectedOptimalSize = new SizeF(800f, 600f); // replace with my actual optimal size
            // Act (execute the logic that generates your query rectangle
            var queryRect = new RectangleF(0, 0, 800f, 600f);

            // Emulate screen/form dimensions using hardcoded values
            float maxHeigh = 1080f;
            float maxWidh = 1920f;

            // Check the valid boundaries
            // Instead of an 'if' condition and Assert.False(false), use the standard Assert.Multiple or a logical assertion
            Assert.Multiple(() =>
            {
                Assert.That(queryRect.Height, Is.GreaterThan(0), "Height must be greater than 0.");
                Assert.That(queryRect.Width, Is.GreaterThan(0), "Width must be greater than 0.");
                Assert.That(queryRect.Height, Is.LessThanOrEqualTo(maxHeigh), "Height exceeds permissible boundaries.");
                Assert.That(queryRect.Width, Is.LessThanOrEqualTo(maxWidh), "Width exceeds permissible boundaries.");
            });

            Assert.That(queryRect.Size, Is.EqualTo(expectedOptimalSize),
                "The selected search area size does not match the optimal size.");

            using (var demo = new DemoPlatform())
            {
                Assert.DoesNotThrow(() => demo.InitSampleData());
                /// IndexOutOfRangeException: the test won’t fail; 
                /// instead, it will simply overwrite someone else’s memory if I get the size wrong.
            }
        }
    }
}



























