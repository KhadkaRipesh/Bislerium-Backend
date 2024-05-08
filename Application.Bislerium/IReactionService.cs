using Domain.Bislerium.DTOs.Reaction;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IReactionService
    {
        public Task<Reaction> AddNewReaction(CreateReaction payload);

        public Task<IEnumerable<Reaction>> GetAllReactions();
    }
}
