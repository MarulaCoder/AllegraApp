using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IMsSqlRepository
    {
        Task<BasicResult> AddNewUser(BasicParameters param);

        Task<BasicResult> AddNewExercise(BasicParameters param);

        Task<BasicResult> GetAllUsers(BasicParameters param);

        Task<BasicResult> GetUserExerciseLog(BasicParameters param);
    }
}
