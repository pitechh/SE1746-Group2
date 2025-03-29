using Business.Domain.Models;
using Business.Resources.Position;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Domain.Services
{
    public interface IPositionService : IBaseService<PositionResource, CreatePositionResource, UpdatePositionResource, Position>
    {
        Task<List<PositionResource>> GetAllPositionsAsync(); 
    }
}