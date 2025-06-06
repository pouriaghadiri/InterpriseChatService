using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class AddDepartmentCommand:IRequest<MessageDTO>
    {
        public EntityName Name{ get; set; }
    }
}
