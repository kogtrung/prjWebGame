using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using WebGame.Models;

namespace WebGame.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error")]
        [Route("Error/{statusCode}")]
        public IActionResult Index(int? statusCode = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            
            var errorViewModel = new ErrorViewModel
            {
                RequestId = requestId
            };

            if (exception != null)
            {
                _logger.LogError(exception, "Request {RequestId} encountered an error", requestId);

                // Database-specific errors
                if (exception is SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "SQL error encountered: {Number}", sqlEx.Number);
                    errorViewModel.Message = "Database error occurred. Please try again later.";
                }
                else if (exception is DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database update error");
                    errorViewModel.Message = "Error updating the database. Please try again later.";
                }
                else
                {
                    errorViewModel.Message = "An unexpected error occurred. Our team has been notified.";
                }
            }
            else if (statusCode.HasValue)
            {
                errorViewModel.StatusCode = statusCode.Value;
                
                switch (statusCode.Value)
                {
                    case 404:
                        errorViewModel.Message = "The page you requested could not be found.";
                        break;
                    case 403:
                        errorViewModel.Message = "You do not have permission to access this resource.";
                        break;
                    case 500:
                        errorViewModel.Message = "Internal server error. Please try again later.";
                        break;
                    default:
                        errorViewModel.Message = $"Error {statusCode.Value} occurred.";
                        break;
                }
                
                _logger.LogWarning("Request {RequestId} returned status code {StatusCode}", 
                    requestId, statusCode.Value);
            }

            return View("Error", errorViewModel);
        }

        [Route("/Error/Database")]
        public IActionResult Database()
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "Database connection error. Please try again later or contact support.",
                StatusCode = 500
            });
        }

        [Route("/Error/NotFound")]
        public IActionResult NotFound()
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "The resource you requested could not be found.",
                StatusCode = 404
            });
        }

        [Route("/Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "You do not have permission to access this resource.",
                StatusCode = 403
            });
        }
    }
} 