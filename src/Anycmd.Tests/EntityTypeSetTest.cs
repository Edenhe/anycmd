﻿
using Anycmd.Util;

namespace Anycmd.Tests
{
    using Ac.ViewModels.Infra.EntityTypeViewModels;
    using Engine.Ac;
    using Engine.Ac.Messages.Infra;
    using Engine.Host.Ac.Infra;
    using Exceptions;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class EntityTypeSetTest
    {
        #region EntityTypeSet
        [Fact]
        public void EntityTypeSet()
        {
            var host = TestHelper.GetAcDomain();
            Assert.Equal(0, host.EntityTypeSet.Count());

            const string codespace = "Test";
            var entityTypeId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();

            EntityTypeState entityTypeById;
            EntityTypeState entityTypeByCode;
            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeId,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = codespace,
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeId, out entityTypeById));
            Assert.True(host.EntityTypeSet.TryGetEntityType(new Coder(codespace, "EntityType1"), out entityTypeByCode));
            Assert.Equal(entityTypeByCode, entityTypeById);
            Assert.True(ReferenceEquals(entityTypeById, entityTypeByCode));

            host.Handle(new UpdateEntityTypeCommand(new EntityTypeUpdateInput
            {
                Id = entityTypeId,
                Name = "test2",
                Code = "EntityType2",
                Codespace = "test",
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeId, out entityTypeById));
            Assert.True(host.EntityTypeSet.TryGetEntityType(new Coder(codespace, "EntityType2"), out entityTypeByCode));
            Assert.Equal(entityTypeByCode, entityTypeById);
            Assert.True(ReferenceEquals(entityTypeById, entityTypeByCode));
            Assert.Equal("test2", entityTypeById.Name);
            Assert.Equal("EntityType2", entityTypeById.Code);

            host.Handle(new RemoveEntityTypeCommand(entityTypeId));
            Assert.False(host.EntityTypeSet.TryGetEntityType(entityTypeId, out entityTypeById));
            Assert.False(host.EntityTypeSet.TryGetEntityType(new Coder(codespace, "EntityType2"), out entityTypeByCode));
            Assert.Equal(0, host.EntityTypeSet.Count());

            // 开始测试Property
            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeId,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = codespace,
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeId, out entityTypeById));
            PropertyState propertyById;
            PropertyState propertyByCode;
            host.Handle(new AddPropertyCommand(new PropertyCreateInput
            {
                Id = propertyId,
                DicId = null,
                ForeignPropertyId = null,
                GuideWords = string.Empty,
                Icon = string.Empty,
                InputType = string.Empty,
                IsDetailsShow = false,
                IsDeveloperOnly = false,
                IsInput = false,
                IsTotalLine = false,
                MaxLength = null,
                EntityTypeId = entityTypeId,
                SortCode = 0,
                Description = string.Empty,
                Code = "Property1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.EntityTypeSet.GetProperties(entityTypeById).Count());
            Assert.True(host.EntityTypeSet.TryGetProperty(propertyId, out propertyById));
            Assert.True(host.EntityTypeSet.TryGetProperty(entityTypeById, "Property1", out propertyByCode));
            Assert.Equal(propertyByCode, propertyById);
            Assert.True(ReferenceEquals(propertyById, propertyByCode));

            host.Handle(new UpdatePropertyCommand(new PropertyUpdateInput
            {
                Id = propertyId,
                Name = "test2",
                Code = "Property2"
            }));
            Assert.Equal(1, host.EntityTypeSet.GetProperties(entityTypeById).Count);
            Assert.True(host.EntityTypeSet.TryGetProperty(propertyId, out propertyById));
            Assert.True(host.EntityTypeSet.TryGetProperty(entityTypeById, "Property2", out propertyByCode));
            Assert.Equal(propertyByCode, propertyById);
            Assert.True(ReferenceEquals(propertyById, propertyByCode));
            Assert.Equal("test2", propertyById.Name);
            Assert.Equal("Property2", propertyById.Code);

            host.Handle(new RemovePropertyCommand(propertyId));
            Assert.False(host.EntityTypeSet.TryGetProperty(propertyId, out propertyById));
            Assert.False(host.EntityTypeSet.TryGetProperty(entityTypeById, "Property2", out propertyByCode));
            Assert.Equal(0, host.EntityTypeSet.GetProperties(entityTypeById).Count);
        }
        #endregion

        #region CanNotDeleteEntityTypeWhenItHasProperties
        [Fact]
        public void CanNotDeleteEntityTypeWhenItHasProperties()
        {
            var host = TestHelper.GetAcDomain();
            Assert.Equal(0, host.EntityTypeSet.Count());

            var entityTypeId = Guid.NewGuid();

            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeId,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = "Test",
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());

            host.Handle(new AddPropertyCommand(new PropertyCreateInput
            {
                Id = Guid.NewGuid(),
                DicId = null,
                ForeignPropertyId = null,
                GuideWords = string.Empty,
                Icon = string.Empty,
                InputType = string.Empty,
                IsDetailsShow = false,
                IsDeveloperOnly = false,
                IsInput = false,
                IsTotalLine = false,
                MaxLength = null,
                EntityTypeId = entityTypeId,
                SortCode = 0,
                Description = string.Empty,
                Code = "Property1",
                Name = "测试1"
            }));

            bool catched = false;
            try
            {
                host.Handle(new RemoveEntityTypeCommand(entityTypeId));
            }
            catch (ValidationException)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                EntityTypeState entityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeId, out entityType));
            }
        }
        #endregion

        #region EntityTypeSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void EntityTypeSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAcDomain();
            Assert.Equal(0, host.EntityTypeSet.Count());

            host.RemoveService(typeof(IRepository<EntityType>));
            var moEntityTypeRepository = host.GetMoqRepository<EntityType, IRepository<EntityType>>();
            var entityId1 = Guid.NewGuid();
            var entityId2 = Guid.NewGuid();
            moEntityTypeRepository.Setup(a => a.Add(It.Is<EntityType>(b => b.Id == entityId1))).Throws(new DbException(entityId1.ToString()));
            moEntityTypeRepository.Setup(a => a.Update(It.Is<EntityType>(b => b.Id == entityId2))).Throws(new DbException(entityId2.ToString()));
            moEntityTypeRepository.Setup(a => a.Remove(It.Is<EntityType>(b => b.Id == entityId2))).Throws(new DbException(entityId2.ToString()));
            moEntityTypeRepository.Setup<EntityType>(a => a.GetByKey(entityId1)).Returns(new EntityType
            {
                Id = entityId1,
                Name = "test1",
                Code = "EntityType1",
                Codespace = "test",
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            });
            moEntityTypeRepository.Setup<EntityType>(a => a.GetByKey(entityId2)).Returns(new EntityType
            {
                Id = entityId2,
                Name = "test2",
                Code = "EntityType2",
                Codespace = "test",
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            });
            host.AddService(typeof(IRepository<EntityType>), moEntityTypeRepository.Object);


            bool catched = false;
            try
            {
                host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
                {
                    Id = entityId1,
                    Code = "EntityType1",
                    Name = "测试1",
                    Codespace = "Test",
                    DatabaseId = host.Rdbs.First().Database.Id,
                    Description = string.Empty,
                    DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                    EditHeight = 100,
                    EditWidth = 100,
                    IsOrganizational = false,
                    SchemaName = string.Empty,
                    SortCode = 10,
                    TableName = string.Empty
                }));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityId1.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.EntityTypeSet.Count());
            }

            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityId2,
                Code = "EntityType2",
                Name = "测试2",
                Codespace = "Test",
                DatabaseId = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateEntityTypeCommand(new EntityTypeUpdateInput
                {
                    Id = entityId2,
                    Name = "test2",
                    Code = "EntityType2",
                    Codespace = "test",
                    DatabaseId = host.Rdbs.First().Database.Id,
                    Description = string.Empty,
                    DeveloperId = host.SysUsers.GetDevAccounts().First().Id,
                    EditWidth = 100,
                    EditHeight = 100,
                    IsOrganizational = false,
                    SchemaName = string.Empty,
                    SortCode = 100,
                    TableName = string.Empty
                }));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityId2.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(1, host.EntityTypeSet.Count());
                EntityTypeState entityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityId2, out entityType));
                Assert.Equal("EntityType2", entityType.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveEntityTypeCommand(entityId2));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityId2.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                EntityTypeState entityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityId2, out entityType));
                Assert.Equal(1, host.EntityTypeSet.Count());
            }
        }
        #endregion
    }
}