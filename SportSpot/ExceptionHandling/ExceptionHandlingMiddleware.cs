﻿using SportSpot.V1.Exceptions;

namespace SportSpot.ExceptionHandling
{
    public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> _logger, RequestDelegate _next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AbstractSportSpotException ex)
            {
                await ex.WriteToResponse(context.Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);
                await new InternalServerErrorException().WriteToResponse(context.Response);
            }
        }
    }
}
