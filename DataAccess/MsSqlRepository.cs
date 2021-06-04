using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MsSqlRepository : IMsSqlRepository
    {
        private readonly AppDbContext _cxt;
        private readonly ILogger<MsSqlRepository> _logger;
        private bool disposed;

        public MsSqlRepository(AppDbContext cxt, ILogger<MsSqlRepository> logger)
        {
            _cxt = cxt;
            _logger = logger;
        }

        public async Task<BasicResult> AddNewUser(BasicParameters param)
        {
            if (param == null)
            {
                return new BasicResult().SetFailure("Input parameter not defined.");
            }

            if (param.User == null)
            {
                return new BasicResult().SetFailure("Input user parameter not defined.");
            }

            var result = new BasicResult();

            try
            {
                var addResult = await _cxt.Users.AddAsync(param.User);

                await _cxt.SaveChangesAsync();

                result.UserId = addResult.Entity.UserId;
                result.SetSuccess();

                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not add new user.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }

        public async Task<BasicResult> GetAllUsers(BasicParameters param)
        {
            var result = new BasicResult();

            if (param == null)
            {
                return result.SetFailure("Input parameter not defined.");
            }

            try
            {
                var users = new List<User>();

                var response = _cxt.Users;

                if (response != null && response.Any())
                {
                    users.AddRange(response);
                }

                // check if any users found
                if (users == null || !users.Any())
                {
                    return result.SetFailure("patients matching not found.");
                }

                result.UserList = users;
                result.SetSuccess();

                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not find matching users.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }

        public async Task<BasicResult> AddNewExercise(BasicParameters param)
        {
            if (param == null)
            {
                return new BasicResult().SetFailure("Input parameter not defined.");
            }

            if (param.Exercise == null)
            {
                return new BasicResult().SetFailure("Input Exercise parameter not defined.");
            }

            var result = new BasicResult();

            try
            {
                var addResult = await _cxt.Exercises.AddAsync(param.Exercise);

                await _cxt.SaveChangesAsync();

                result.ExerciseId = addResult.Entity.ExerciseId;
                result.SetSuccess();

                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not add new user.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }

        public async Task<BasicResult> GetUserExerciseLog(BasicParameters param)
        {
            var result = new BasicResult();

            if (param == null)
            {
                return result.SetFailure("Input parameter not defined.");
            }

            try
            {
                var exercises = _cxt.Exercises.Where(x => x.UserId == param.UserId).ToList();

                // check if username exists
                if (exercises == null || !exercises.Any())
                {
                    return result.SetFailure($"user [{param.UserId}] exercise logs not found.");
                }


                // authentication successful
                result.ExerciseList = exercises;
                result.SetSuccess();

                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not find user exercise logs.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }
    }
}
