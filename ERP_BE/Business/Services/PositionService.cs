using AutoMapper;
using Business.Communication;
using Business.CustomException;
using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Resources;
using Business.Resources.Position;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public class PositionService : BaseService<PositionResource, CreatePositionResource, UpdatePositionResource, Position>, IPositionService
    {
        #region Property
        private readonly IPositionRepository _positionRepository;
        #endregion

        #region Constructor
        public PositionService(IPositionRepository positionRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<ResponseMessage> responseMessage) : base(positionRepository, mapper, unitOfWork, responseMessage)
        {
            this._positionRepository = positionRepository;
        }
        #endregion

        #region Method
        public async Task<List<PositionResource>> GetAllPositionsAsync()
        {
            var positions = await _positionRepository.GetAllAsync();
            return Mapper.Map<List<PositionResource>>(positions);
        }

        public override async Task<BaseResponse<PositionResource>> InsertAsync(CreatePositionResource createPositionResource)
        {
            try
            {
                // Validate position name is existent?
                var hasValue = await _positionRepository.FindByNameAsync(createPositionResource.Name, true);
                if (hasValue.Count > 0)
                    return new BaseResponse<PositionResource>(ResponseMessage.Values["Position_Existent"]);

                // Mapping Resource to Position
                var position = Mapper.Map<CreatePositionResource, Position>(createPositionResource);

                await _positionRepository.InsertAsync(position);
                await UnitOfWork.CompleteAsync();

                return new BaseResponse<PositionResource>(Mapper.Map<Position, PositionResource>(position));
            }
            catch (Exception ex)
            {
                throw new MessageResultException(ResponseMessage.Values["Position_Saving_Error"], ex);
            }
        }
        #endregion
    }
}
