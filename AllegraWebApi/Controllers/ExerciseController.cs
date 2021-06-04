using AllegraApi.Models;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AllegraApi.Controllers
{
    [Route("api/exercise/")]
    public class ExerciseController : Controller
    {
        private readonly ILogger<ExerciseController> _logger;
        private readonly IBusinessOperations _bizOps;
        public ExerciseController(ILogger<ExerciseController> logger, IBusinessOperations bizOps)
        {
            _logger = logger;
            _bizOps = bizOps;
        }

        // add a new user
        [HttpPost]
        [Route("new-user", Name = "AddUser")]
        [SwaggerResponse(statusCode: 200, description: "User sucessfully created.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ServiceApiResponse), description: "Bad request. Check user model item body json.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ServiceApiResponse), description: "Unauthorized to perform action.")]
        [SwaggerResponse(statusCode: 403, type: typeof(ServiceApiResponse), description: "Forbidden to peform action.")]
        [SwaggerResponse(statusCode: 409, type: typeof(ServiceApiResponse), description: "User with similar attributes already exists.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ServiceApiResponse), description: "Internal server error.")]
        public async Task<object> AddUser(
            [FromBody][Required] UserRequestModel userModel,
            [FromHeader][Required] string traceId,
            [FromHeader][Required] string jwtSecToken,
            [FromHeader] DateTime requestDate
            )
        {
            try
            {
                //TODO:validation
                if (string.IsNullOrEmpty(userModel.Username))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return new ServiceApiResponse
                    {
                        Title = "Service Error",
                        ReasonPhrase = "Username cannot be null.",
                        ErrorType = "Validation Error",
                        Status = Response.StatusCode,
                        TraceId = traceId
                    };
                }

                var user = new User()
                {
                    Username = userModel.Username
                };


                //process
                _logger.LogDebug($"{traceId} - add new user - exerciseController.bizOps");
                var addResult = await _bizOps.AddNewUser(new BasicParameters()
                {
                    User = user
                });
                if (!addResult.Successful)
                {
                    if (!addResult.ValidationError)
                    {
                        addResult.SetFailure("Internal server error.");
                    }

                    if (addResult.EntityExists)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return new ServiceApiResponse
                        {
                            Title = "Service Error",
                            ReasonPhrase = addResult.Reason,
                            ErrorType = "Validation Error",
                            Status = Response.StatusCode,
                            TraceId = traceId
                        };
                    }

                    Response.StatusCode = addResult.ValidationError ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
                    return new ServiceApiResponse
                    {
                        Title = "Service Error",
                        ReasonPhrase = addResult.Reason,
                        ErrorType = "Error",
                        Status = Response.StatusCode,
                        TraceId = traceId
                    };
                }

                return Ok(new { userId = addResult.UserId });
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"{traceId} - failed to add new user - exerciseController.bizOps");
                _logger.LogError($"{ex.Message}");

                // return ApiResponse
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new ServiceApiResponse
                {
                    Title = "Service Error",
                    ReasonPhrase = "Internal server error.",
                    ErrorType = "Error",
                    Status = Response.StatusCode,
                    TraceId = traceId
                };
            }
        }

        // add a new use exercise log
        [HttpPost]
        [Route("add", Name = "AddExercise")]
        [SwaggerResponse(statusCode: 200, description: "Exercise sucessfully created.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ServiceApiResponse), description: "Bad request. Check Exercise detail item body json.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ServiceApiResponse), description: "Unauthorized to perform action.")]
        [SwaggerResponse(statusCode: 403, type: typeof(ServiceApiResponse), description: "Forbidden to peform action.")]
        [SwaggerResponse(statusCode: 409, type: typeof(ServiceApiResponse), description: "Exercise with similar attributes already exists.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ServiceApiResponse), description: "Internal server error.")]
        public async Task<object> AddExercise(
            [FromBody][Required] ExerciseRequestModel exerciseItem,
            [FromHeader][Required] string traceId,
            [FromHeader][Required] string jwtSecToken,
            [FromHeader] DateTime requestDate
            )
        {
            try
            {
                //TODO:validation

                var exercise = new Exercise()
                {
                    UserId = exerciseItem.UserId,
                    Description = exerciseItem.Description,
                    Duration = exerciseItem.Duration,
                    Date = exerciseItem.Date
                };

                //process
                _logger.LogDebug($"{traceId} - add new Exercise - exerciseController.bizOps");
                var addResult = await _bizOps.AddNewExercise(new BasicParameters()
                {
                    Exercise = exercise
                });
                if (!addResult.Successful)
                {
                    if (!addResult.ValidationError)
                    {
                        addResult.SetFailure("Internal server error.");
                    }

                    if (addResult.EntityExists)
                    {
                        Response.StatusCode = (int)HttpStatusCode.Conflict;
                        return new ServiceApiResponse
                        {
                            Title = "Service Error",
                            ReasonPhrase = addResult.Reason,
                            ErrorType = "Validation Error",
                            Status = Response.StatusCode,
                            TraceId = traceId
                        };
                    }

                    Response.StatusCode = addResult.ValidationError ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
                    return new ServiceApiResponse
                    {
                        Title = "Service Error",
                        ReasonPhrase = addResult.Reason,
                        ErrorType = "Error",
                        Status = Response.StatusCode,
                        TraceId = traceId
                    };
                }

                return Ok(new { exerciseId = addResult.ExerciseId });
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"{traceId} - failed to add new exercise - exerciseController.bizOps");
                _logger.LogError($"{ex.Message}");

                // return ApiResponse
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new ServiceApiResponse
                {
                    Title = "Service Error",
                    ReasonPhrase = "Internal server error.",
                    ErrorType = "Error",
                    Status = Response.StatusCode,
                    TraceId = traceId
                };
            }
        }

        // get  all users
        [HttpGet]
        [Route("users", Name = "GetAllUsers")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<UserResponseModel>), description: "Users sucessfully retrieved.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ServiceApiResponse), description: "Bad request.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ServiceApiResponse), description: "Unauthorized to perform action.")]
        [SwaggerResponse(statusCode: 403, type: typeof(ServiceApiResponse), description: "Forbidden to peform action.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ServiceApiResponse), description: "Internal server error.")]
        public async Task<object> GetAllUserss(
            [FromHeader][Required] string traceId,
            [FromHeader][Required] string jwtSecToken,
            [FromHeader] DateTime requestDate
            )
        {
            try
            {
                //TODO:validation

                //process
                _logger.LogDebug($"{traceId} - get all users - exerciseController.bizOps");
                var getResult = await _bizOps.GetAllUsers(new BasicParameters());
                if (!getResult.Successful)
                {
                    if (!getResult.ValidationError)
                    {
                        getResult.SetFailure("Internal server error.");
                    }

                    if (getResult.UserList == null || !getResult.UserList.Any())
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return new ServiceApiResponse
                        {
                            Title = "Service Error",
                            ReasonPhrase = getResult.Reason,
                            ErrorType = "Validation Error",
                            Status = Response.StatusCode,
                            TraceId = traceId
                        };
                    }

                    Response.StatusCode = getResult.ValidationError ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
                    return new ServiceApiResponse
                    {
                        Title = "Service Error",
                        ReasonPhrase = getResult.Reason,
                        ErrorType = "Error",
                        Status = Response.StatusCode,
                        TraceId = traceId
                    };
                }

                var responseObj = getResult.UserList.Select(user => new UserResponseModel()
                {
                    UserId = user.UserId,
                    Username = user.Username
                }).ToList();

                return Ok(responseObj);
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"{traceId} - failed to get all users - exerciseController.bizOps");
                _logger.LogError($"{ex.Message}");

                // return ApiResponse
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new ServiceApiResponse
                {
                    Title = "Service Error",
                    ReasonPhrase = "Internal server error.",
                    ErrorType = "Error",
                    Status = Response.StatusCode,
                    TraceId = traceId
                };
            }
        }

        // get user exercise logs
        [HttpGet]
        [Route("log", Name = "GetExerciseLogs")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ExerciseResponseModel>), description: "Exercise logs sucessfully retrieved.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ServiceApiResponse), description: "Bad request.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ServiceApiResponse), description: "Unauthorized to perform action.")]
        [SwaggerResponse(statusCode: 403, type: typeof(ServiceApiResponse), description: "Forbidden to peform action.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ServiceApiResponse), description: "Internal server error.")]
        public async Task<object> GetExerciseLogs(
            [FromQuery][Required] int userId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int limit,
            [FromHeader][Required] string traceId,
            [FromHeader][Required] string jwtSecToken,
            [FromHeader] DateTime requestDate
            )
        {
            try
            {
                //TODO:validation

                //process
                _logger.LogDebug($"{traceId} - get user exercise logs - exerciseController.bizOps");
                var getResult = await _bizOps.GetUserExerciseLog(new BasicParameters()
                {
                    UserId = userId,
                    From = from,
                    To = to,
                    Limit = limit
                });
                if (!getResult.Successful)
                {
                    if (!getResult.ValidationError)
                    {
                        getResult.SetFailure("Internal server error.");
                    }

                    if (getResult.ExerciseList == null || !getResult.ExerciseList.Any())
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return new ServiceApiResponse
                        {
                            Title = "Service Error",
                            ReasonPhrase = getResult.Reason,
                            ErrorType = "Validation Error",
                            Status = Response.StatusCode,
                            TraceId = traceId
                        };
                    }

                    Response.StatusCode = getResult.ValidationError ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
                    return new ServiceApiResponse
                    {
                        Title = "Service Error",
                        ReasonPhrase = getResult.Reason,
                        ErrorType = "Error",
                        Status = Response.StatusCode,
                        TraceId = traceId
                    };
                }

                var responseObj = getResult.ExerciseList.Select(exercise => new ExerciseResponseModel()
                {
                    ExerciseId = exercise.ExerciseId,
                    UserId = exercise.UserId,
                    Description = exercise.Description,
                    Duration = exercise.Duration,
                    Date = exercise.Date

                }).ToList();

                return Ok(responseObj);
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError($"{traceId} - failed to user exercise logs - exerciseController.bizOps");
                _logger.LogError($"{ex.Message}");

                // return ApiResponse
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new ServiceApiResponse
                {
                    Title = "Service Error",
                    ReasonPhrase = "Internal server error.",
                    ErrorType = "Error",
                    Status = Response.StatusCode,
                    TraceId = traceId
                };
            }
        }
    }
}
