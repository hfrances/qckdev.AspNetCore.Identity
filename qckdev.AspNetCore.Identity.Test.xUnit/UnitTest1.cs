using MediatR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace qckdev.AspNetCore.Identity.Test.xUnit
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x =>
                    x.Send(It.IsAny<qckdev.AspNetCore.Identity.Commands.LoginCommand>(), default))
                        .ReturnsAsync(new qckdev.AspNetCore.Identity.ViewModels.TokenViewModel()
                        {
                            AccessToken = "Patata"
                        });

            var request = new qckdev.AspNetCore.Identity.Commands.LoginCommand() { Email = "test@miauth.loc" };
            var controller = new qckdev.AspNetCore.Identity.Controllers.AuthController(mediator.Object);
            await controller.LoginAsync(request);

            mediator
                .Verify(x =>
                    x.Send(It.Is<qckdev.AspNetCore.Identity.Commands.LoginCommand>(y => y.Email == request.Email), default), 
                        Times.Once);
        }
    }
}
