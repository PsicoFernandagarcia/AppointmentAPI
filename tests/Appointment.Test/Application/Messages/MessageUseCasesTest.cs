using Appointment.Application.MessageUseCases.CreateMessage;
using Appointment.Application.MessageUseCases.DeleteMessage;
using Appointment.Application.MessageUseCases.GetMessage;
using Appointment.Application.MessageUseCases.UpdateMessage;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Appointment.Test.Application.Messages
{
    public class MessageUseCasesTest
    {
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
        private readonly Mock<IOutputCacheStore> _cacheStoreMock = new();

        [Fact]
        public async Task CreateMessage_Should_Insert_And_Invalidate_Cache()
        {
            var command = new CreateMessageCommand("Welcome", "Hello world", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(2), true);
            var createdMessage = Message.Create(command.Title, command.Content, command.DateFrom, command.DateTo, command.Active).Value;

            _messageRepositoryMock.Setup(r => r.Insert(It.IsAny<Message>()))
                .ReturnsAsync(createdMessage);

            var handler = new CreateMessageHandler(_messageRepositoryMock.Object, _cacheStoreMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Welcome");
            result.Value.Content.Should().Be("Hello world");

            _messageRepositoryMock.Verify(r => r.Insert(It.IsAny<Message>()), Times.Once);
            _cacheStoreMock.Verify(c => c.EvictByTagAsync(CacheKeys.Messages, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_Should_Update_And_Invalidate_Cache_When_Exists()
        {
            var existing = Message.Create("Old Title", "Old Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1)).Value;
            _messageRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(existing);
            _messageRepositoryMock.Setup(r => r.Update(It.IsAny<Message>())).ReturnsAsync(existing);

            var command = new UpdateMessageCommand(1, "New Title", "New Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(5), false);
            var handler = new UpdateMessageHandler(_messageRepositoryMock.Object, _cacheStoreMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("New Title");
            result.Value.Active.Should().BeFalse();

            _messageRepositoryMock.Verify(r => r.Update(It.IsAny<Message>()), Times.Once);
            _cacheStoreMock.Verify(c => c.EvictByTagAsync(CacheKeys.Messages, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_Should_Return_DoesNotExistError_When_NotFound()
        {
            _messageRepositoryMock.Setup(r => r.Get(99)).ReturnsAsync((Message)null);

            var command = new UpdateMessageCommand(99, "Title", "Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1), true);
            var handler = new UpdateMessageHandler(_messageRepositoryMock.Object, _cacheStoreMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteMessage_Should_Delete_Entity_And_Invalidate_Cache()
        {
            var existing = Message.Create("Title", "Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1), true).Value;
            _messageRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(existing);
            _messageRepositoryMock.Setup(r => r.Delete(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var command = new DeleteMessageCommand(1);
            var handler = new DeleteMessageHandler(_messageRepositoryMock.Object, _cacheStoreMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            _messageRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
            _cacheStoreMock.Verify(c => c.EvictByTagAsync(CacheKeys.Messages, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetMessageById_Should_Return_Dto_When_Exists()
        {
            var existing = Message.Create("Title", "Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1)).Value;
            _messageRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(existing);

            var query = new GetMessageByIdQuery(1);
            var handler = new GetMessageByIdHandler(_messageRepositoryMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Title");
        }

        [Fact]
        public async Task GetMessages_Should_Return_List_From_Repo()
        {
            var msg1 = Message.Create("T1", "C1", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1)).Value;
            var msg2 = Message.Create("T2", "C2", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(2)).Value;
            _messageRepositoryMock.Setup(r => r.Get(false)).ReturnsAsync(new List<Message> { msg1, msg2 });

            var query = new GetMessagesQuery { ShowAll = false };
            var handler = new GetMessagesHandler(_messageRepositoryMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }
}
