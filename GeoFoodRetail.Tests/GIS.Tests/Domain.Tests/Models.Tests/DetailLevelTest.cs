/**
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Linq;
using GeoFoodRetail.GIS.Domain.Models;


namespace GeoFoodRetail.Tests.GIS.Tests.Domain.Tests.Models.Tests
{
    public class DetailLevelTest
    {
        /* If general data preparation is needed for tests in the future, 
         * the method `public void [Test]Setup()` can always be restored.*/

        [Test]
        public void DetailLevel_EnumValues_ExistAndHaveCorrectDefaults()
        {
            // Checking for the presence of expected values ​​in the DetailLevel enumeration
            Assert.That(System.Enum.IsDefined(typeof(DetailLevel), "Clustered"), Is.True);
            Assert.That(System.Enum.IsDefined(typeof(DetailLevel), "Detailed"), Is.True);
        }

        [Test]
        public void DetailLevel_ExplicitCasting_WorksCorrectly()
        {
            // Checking integer values ​​of an enumeration
            DetailLevel clustered = DetailLevel.Clustered;
            DetailLevel detailed = DetailLevel.Detailed;

            Assert.That((int)clustered, Is.Not.EqualTo((int)detailed));
        }
    }
}
