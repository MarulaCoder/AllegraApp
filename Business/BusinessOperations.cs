using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class BusinessOperations : IBusinessOperations
    {
        private readonly ILogger<BusinessOperations> _logger;
        private readonly IMsSqlRepository _repo;
        private readonly ICacheManager _cacheManager;

        public BusinessOperations(ILogger<BusinessOperations> logger, IMsSqlRepository repo, ICacheManager cacheManager)
        {
            _logger = logger;
            _repo = repo;
            _cacheManager = cacheManager;
        }

        public async Task<BasicResult> AddNewUser(BasicParameters param)
        {
            var result = new BasicResult();

            if (param == null)
            {
                return result.SetFailure("Input parameter not defined.");
            }

            if (param.User == null)
            {
                return new BasicResult().SetFailure("Input User parameter not defined.");
            }

            try
            {
                var addResult = await _repo.AddNewUser(param);
                if (!addResult.Successful)
                {
                    _logger.LogError($"Error: could not add new user.");
                    _logger.LogError($"{addResult.Reason}");

                    if (!addResult.ValidationError)
                    {
                        return result.SetFailure("Internal server error.");
                    }

                    return result.SetFailure(addResult.Reason);
                }

                //update cache
                await _cacheManager.RefreshCache("_user_list");

                result.UserId = addResult.UserId;
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

                // get patients from cache
                var key = "_user_list";
                var users = await _cacheManager.GetCached<IEnumerable<User>>(key);

                if (users == null || !users.Any())
                {
                    // get exercises from database
                    var getResult = await _repo.GetAllUsers(param);
                    if (!getResult.Successful)
                    {
                        _logger.LogError($"Error: could not get all users.");
                        _logger.LogError($"{getResult.Reason}");

                        if (!getResult.ValidationError)
                        {
                            return result.SetFailure("Internal server error.");
                        }

                        return result.SetFailure(getResult.Reason);
                    }

                    users = getResult.UserList;
                    if (users != null && users.Any())
                    {
                        await _cacheManager.SetCache(key, users, TimeSpan.FromHours(2));
                    }
                }

                result.UserList = users.ToList();
                result.SetSuccess();
                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not get all users.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }

        public async Task<BasicResult> AddNewExercise(BasicParameters param)
        {
            var result = new BasicResult();

            if (param == null)
            {
                return result.SetFailure("Input parameter not defined.");
            }

            if (param.Exercise == null)
            {
                return new BasicResult().SetFailure("Input User parameter not defined.");
            }

            try
            {
                var addResult = await _repo.AddNewExercise(param);
                if (!addResult.Successful)
                {
                    _logger.LogError($"Error: could not add new exercise log.");
                    _logger.LogError($"{addResult.Reason}");

                    if (!addResult.ValidationError)
                    {
                        return result.SetFailure("Internal server error.");
                    }

                    return result.SetFailure(addResult.Reason);
                }

                //update cache
                await _cacheManager.RefreshCache("_exercise_log_" + param.UserId);

                result.UserId = addResult.UserId;
                result.SetSuccess();
                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not add new exercise log.");
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
                // get exercises from cache
                var key = "_exercise_log_" + param.UserId;
                var exercises = await _cacheManager.GetCached<IEnumerable<Exercise>>(key);

                if (exercises == null || !exercises.Any())
                {
                    // get patients from database
                    var getResult = await _repo.GetUserExerciseLog(param);
                    if (!getResult.Successful)
                    {
                        _logger.LogError($"Error: could not get user exercises.");
                        _logger.LogError($"{getResult.Reason}");

                        if (!getResult.ValidationError)
                        {
                            return result.SetFailure("Internal server error.");
                        }

                        return result.SetFailure(getResult.Reason);
                    }

                    exercises = getResult.ExerciseList;
                    if (exercises != null && exercises.Any())
                    {
                        await _cacheManager.SetCache(key, exercises, TimeSpan.FromHours(2));
                    }
                }

                result.ExerciseList = exercises.Where(x => x.Date >= param.From && x.Date <= param.To).Take(param.Limit).ToList();
                result.SetSuccess();
                return result;
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"Error: could not get user exercises.");
                _logger.LogError($"{ex.Message}");

                return result.SetFailure($"Internal server error.");
            }
        }
    }
}
