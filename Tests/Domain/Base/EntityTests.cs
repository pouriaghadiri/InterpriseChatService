using Domain.Base;
using Domain.Base.Interface;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Base
{
    public class EntityTests
    {
        [Fact]
        public void Entity_Should_Have_Valid_Default_Properties()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Id.Should().NotBeEmpty();
            entity.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            entity.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            entity.CreatedBy.Should().Be(0);
            entity.ModifiedBy.Should().Be(0);
            entity.IsActive.Should().BeTrue();
            entity.IsDeleted.Should().BeFalse();
            entity.DeletedAt.Should().Be(default(DateTime));
            entity.IsArchived.Should().BeFalse();
        }

        [Fact]
        public void Update_Should_Update_UpdatedAt_Timestamp()
        {
            // Arrange
            var entity = new TestEntity();
            var originalUpdatedAt = entity.UpdatedAt;

            // Wait a small amount to ensure time difference
            Thread.Sleep(10);

            // Act
            entity.Update();

            // Assert
            entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
            entity.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Delete_Should_Set_Deletion_Properties()
        {
            // Arrange
            var entity = new TestEntity();
            var originalUpdatedAt = entity.UpdatedAt;

            // Wait a small amount to ensure time difference
            Thread.Sleep(10);

            // Act
            entity.Delete();

            // Assert
            entity.IsActive.Should().BeFalse();
            entity.IsDeleted.Should().BeTrue();
            entity.DeletedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void DeActive_Should_Set_IsActive_To_False()
        {
            // Arrange
            var entity = new TestEntity();
            var originalUpdatedAt = entity.UpdatedAt;

            // Wait a small amount to ensure time difference
            Thread.Sleep(10);

            // Act
            entity.DeActive();

            // Assert
            entity.IsActive.Should().BeFalse();
            entity.IsDeleted.Should().BeFalse(); // Should not affect deletion status
            entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void Activate_Should_Set_IsActive_To_True()
        {
            // Arrange
            var entity = new TestEntity();
            entity.DeActive(); // First deactivate
            var originalUpdatedAt = entity.UpdatedAt;

            // Wait a small amount to ensure time difference
            Thread.Sleep(10);

            // Act
            entity.Activate();

            // Assert
            entity.IsActive.Should().BeTrue();
            entity.IsDeleted.Should().BeFalse();
            entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void Multiple_Updates_Should_Update_Timestamp_Each_Time()
        {
            // Arrange
            var entity = new TestEntity();
            var firstUpdate = entity.UpdatedAt;

            // Wait a small amount
            Thread.Sleep(10);

            // Act
            entity.Update();
            var secondUpdate = entity.UpdatedAt;

            Thread.Sleep(10);
            entity.Update();
            var thirdUpdate = entity.UpdatedAt;

            // Assert
            thirdUpdate.Should().BeAfter(secondUpdate);
            secondUpdate.Should().BeAfter(firstUpdate);
        }

        [Fact]
        public void Delete_After_DeActive_Should_Still_Work()
        {
            // Arrange
            var entity = new TestEntity();
            entity.DeActive();

            // Act
            entity.Delete();

            // Assert
            entity.IsActive.Should().BeFalse();
            entity.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Activate_After_Delete_Should_Not_Affect_Deletion_Status()
        {
            // Arrange
            var entity = new TestEntity();
            entity.Delete();

            // Act
            entity.Activate();

            // Assert
            entity.IsActive.Should().BeTrue();
            entity.IsDeleted.Should().BeTrue(); // Deletion status should remain
        }

        [Fact]
        public void Entity_Should_Generate_Unique_Ids()
        {
            // Arrange & Act
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();
            var entity3 = new TestEntity();

            // Assert
            entity1.Id.Should().NotBe(entity2.Id);
            entity2.Id.Should().NotBe(entity3.Id);
            entity1.Id.Should().NotBe(entity3.Id);
        }

        [Fact]
        public void Entity_Should_Implement_IEntity_Interface()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Should().BeAssignableTo<IEntity>();
        }

        [Fact]
        public void Entity_Should_Have_Consistent_Timestamps_On_Creation()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.CreatedAt.Should().BeCloseTo(entity.UpdatedAt, TimeSpan.FromMilliseconds(100));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Multiple_Entities_Should_Have_Different_Timestamps(int count)
        {
            // Arrange & Act
            var entities = new List<TestEntity>();
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(1); // Small delay to ensure different timestamps
                entities.Add(new TestEntity());
            }

            // Assert
            var timestamps = entities.Select(e => e.CreatedAt).ToList();
            timestamps.Should().OnlyHaveUniqueItems();
        }

        // Helper class for testing Entity
        private class TestEntity : Entity
        {
            // Empty implementation for testing base Entity functionality
        }
    }
}

