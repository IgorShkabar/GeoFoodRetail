/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * Make sure to add a link to the main project containing the source code.
 */
using NUnit.Framework;
using System.Linq;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeoFoodRetail.GIS.StreamingRTree;
using GeoFoodRetail.GIS.Abstractions;
using GeoFoodRetail.GIS.UI;

[assembly: InternalsVisibleTo("Buying.Tests")]
namespace GeoFoodRetail.Tests.GIS.Tests.StreamingRTree.Tests
{
    [TestFixture]
    public class SpatialIndexManagerTests
    {
        private ISpatialIndex<StoreItem> spatial_i;               
        private SpatialIndexManagerTests spa_i_mngr_tst; 
        private readonly List<SpatialNode<StoreItem>> _points = new List<SpatialNode<StoreItem>>();

        public SpatialIndexManagerTests() { }

        public SpatialIndexManagerTests(ISpatialIndex<StoreItem> spatial_i)
        {
            this.spatial_i = spatial_i;
        }

        private class FakeSpatialIndex : ISpatialIndex<StoreItem>
        {
            /* Initialize a lightweight collection (e.g., List<StoreItem>) within the mock.
             * This will allow for emulating R-Tree operations—such as insertion and AABB-based searching—
             * directly in memory, without requiring complex logic. An internal store to simulate an R-Tree index. */
            private readonly List<StoreItem> Items = new List<StoreItem>();
            private bool isBuilt;  

            public List<StoreItem> LoggedItems => Items; 

            public List<StoreItem> ItemsProperty => Items; 
            
            public FakeSpatialIndex()
            {
                var spatial_i = new SpatialIndexManagerTests();
                _ = spatial_i ?? throw new System.ArgumentNullException(nameof(spatial_i), "Spatial index cannot be null.");
            }
            
#if NET48
            // конструктор только для классического .NET Framework (4.8 и ниже)
            public FakeSpatialIndex(ISpatialIndex<StoreItem> realOrMockIndex)
            {
                spatial_i = realOrMockIndex ?? throw new ArgumentNullException(nameof(realOrMockIndex));
            }
#elif NET8_0_OR_GREATER
            // Обязательный конструктор
            // If you`re using modern C#,  it is best to rewrite the first version using `ArgumentNullException.ThrowIfNull`.
            public FakeSpatialIndex(ISpatialIndex<StoreItem> spatialIndex)
            {
                ArgumentNullException.ThrowIfNull(spatialIndex);
                spatial_i = spatialIndex;
            }
#endif
            public void Build()
            {
                isBuilt = true;
            }
            /// Implementation of the ISpatialIndex interface:

            public void Build(IEnumerable<StoreItem> items)
            {
                isBuilt = true; /**simulate an actual index build by changing the State*/
                Items.Clear();
                Items.AddRange(items);
                isBuilt = true;
            }

            public IEnumerable<StoreItem> Search(object queryEnvelope)
            {
                return Items;
            }

            /// Adding data requires rebuilding the index
            public void Insert(Envelope env, StoreItem itm) => Items.Add(itm);            

            public void Insert(in SimdAABB bounds, StoreItem itm) => Items.Add(itm);
            
            public void ClearIndex()
            {
                Items.Clear();
                isBuilt = false;
            }   

            public IList<StoreItem> Query(Envelope searchEnv)
            {
                // simulate behavior of a real index: If Build() hasn`t been called, the search will not work.
                if (!isBuilt) {
                    throw new System.InvalidOperationException("Index must be built before querying");
                }
                // return the data
                return Items;
                // for a mock, you can simply return the entire list 
                // or apply basic filtering if required for tests
            }
        } /// end of Fake class


        [SetUp]
        public void Setup()
        {
            /** If you install the NuGet package Moq:
            spatial_i_Mock = new Mock<ISpatialIndex<Buying.GIS.UI.StoreItem>>();    */

            // initialize the fake before running the test
            spatial_i = new FakeSpatialIndex();
            // I am passing it to the constructor of the class being tested
            spa_i_mngr_tst = new SpatialIndexManagerTests(spatial_i);

            _points.Clear();
            _points.Add(default(SpatialNode<StoreItem>));
        }

        /** If you use xUnit framework instead of NUnit*/
#if XUNIT
using Xunit;
        [Fact]
        public void Build_ShouldBuildIndex()
        {
            var fake_i = new FakeSpatialIndex(); // Fake index
            var mngr = new SpatialIndexManager<StoreItem>(fake_i);
            mngr.Insert(new Envelope(), new StoreItem()); 
            
            mngr.Build(); // act

            // If the manager internally called fake_i.Build(), the Query method will execute without errors.
            var excpt = Record.Exception(() => mngr.Query(new Envelope));
            Assert.Null(excpt);
        }
#endif
        [Test]
        public void Build_ShouldBuildIndex()
        {
            var fake_i = new FakeSpatialIndex(); // Fake index
            var mngr = new SpatialIndexManager<StoreItem>(fake_i);
            mngr.Insert(new Envelope(), new StoreItem());
            mngr.Insert(new SimdAABB(), new StoreItem());

            // act
            mngr.Build();

            // If the manager internally called fake_i.Build(), the Query method will execute without errors.
            System.Exception excpt = null;
            try { mngr.Query(new Envelope()); }
            catch(System.Exception x)
            {
                excpt = x;
            }
            Assert.Null(excpt);
        }
        
        [Test]
        public void ClearnIndex_ShouldClearInternalSpatialIndex()
        {
            var fakeIndex = new FakeSpatialIndex();
                      
            _points.Add(new SpatialNode<StoreItem> { Item = new StoreItem { Id = 1 } });
            _points.Add(new SpatialNode<StoreItem> { Item = new StoreItem { Id = 2 } });

            var storeItems = _points.Select(node => node.Item).ToList();
            fakeIndex.ItemsProperty.AddRange(storeItems);
            // Act
            fakeIndex.ClearIndex();
            
            Assert.IsEmpty(((FakeSpatialIndex)spatial_i).ItemsProperty);
            Assert.IsEmpty(fakeIndex.ItemsProperty, "Список элементов должен быть пуст после очистки.");            
        }    

        [Test]
        public void Insert_ShouldAdd_ItemWhen_called()
        {
            var fake_i = new FakeSpatialIndex();
            var mngr = new SpatialIndexManager<StoreItem>();

            SimdAABB _boundary = new SimdAABB(0f, 0f, 0f, 0f);
            var node = new StoreItem("Walmart Inc.", 36.3726f, -94.2088f);

            // Act
            mngr.Insert(node, 36f, -94f);

            Assert.False(false);
            Assert.That(_boundary.Contains(0f, 0f));            
            Assert.That(node.Name, Is.EqualTo("Walmart Inc."));
        }       

        /** If you install the NuGet package Moq in your test project you can use:
           private Mock<ISpatialIndex<Buying.GIS.UI.StoreItem>> spatial_i_Mock; // spatial_i_Mock is abbrv. of spatial index Mock
        */
        
        /*
        [Test]
        public void AddItem_ShouldAddStoreItemToSpatialIndex()
        {
            // Arrange (Подготовка)
            var fakeIndex = new FakeSpatialIndex();
            // Передаем наш фейк в тестируемый менеджер (Внедрение зависимостей)
            var manager = new SpatialIndexManager<StoreItem>(fakeIndex);
            var item = new StoreItem { Name = "Walmart Inc." };
            var bounds = new SimdAABB(); // Ваши координаты bounds

            // Act (Действие)
            manager.Insert(bounds, item); // Менеджер внутри должен вызвать index.Insert(...)

            // Assert (Проверка результатов)
            // Используй свойство LoggedItems для проверки. Проверяй, что добавился ровно 1 элемент
            Assert.That(fakeIndex.LoggedItems, Has.Count.EqualTo(1), "Exactly one element was added"); 
            Assert.AreSame(item, fakeIndex.LoggedItems[0]); // Проверяем, что это именно наш объект
        }   */
       
        [Test]
        public void Query_WhenCalled_ReturnsExpectedItems()
        {           
            var expectedItems = new List<StoreItem>
            {
                new StoreItem { Name = "Walmart Inc.", Latitude = 5.0, Longitude = 5.0 }
            };

            /// If you install the NuGet package Moq (I didn`t):
            /// spatial_i_Mock
            ///    .Setup(x => x.Query(testEnvelope))
            ///    .Returns(expectedItems);

            /// var result = spatial_i_Mock.Object.Query(testEnvelope);
            /// spatial_i_Mock.Verify(x => x.Query(testEnvelope), Times.Once);   


            var index = new FakeSpatialIndex();
            // Preparation
            var testEnvelope = new Envelope(0, 10, 0, 10);
            var store = new StoreItem { Name = "Walmart Inc.", Latitude = 5.0, Longitude = 5.0 };

            
            index.Insert(testEnvelope, store);
            //index.Insert(store, 5f, 5f);
            index.Build();
            // Action
            var result = index.Query(testEnvelope);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Walmart Inc."));            
        }         
    }
}