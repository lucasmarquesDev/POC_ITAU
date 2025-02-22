using AutoMapper;
using POC_ITAU.Domain.Entities.Request;

namespace POC_ITAU.Application.UseCases.CreateNotificarion
{
    public class CreateNotificarionMapper : Profile
    {
        public CreateNotificarionMapper()
        {
            CreateMap<CreateNotificarionRequest, Notification>();
        }
    }
}
