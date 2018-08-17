using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessMicroservices.Models;
using ServerlessMicroservices.Shared.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerlessMicroservices.FunctionApp.Trips
{
    public static class TripFunctions
    {
        [FunctionName("GetTrips")]
        public static async Task<IActionResult> GetTrips([HttpTrigger(AuthorizationLevel.Function, "get", Route = "trips")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetTrips triggered....");

            try
            {
                var persistenceService = ServiceFactory.GetPersistenceService();
                return (ActionResult)new OkObjectResult(await persistenceService.RetrieveTrips());
            }
            catch (Exception e)
            {
                var error = $"GetTrips failed: {e.Message}";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }
        }

        [FunctionName("GetActiveTrips")]
        public static async Task<IActionResult> GetActiveTrips([HttpTrigger(AuthorizationLevel.Function, "get", Route = "activetrips")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetActiveTrips triggered....");

            try
            {
                var persistenceService = ServiceFactory.GetPersistenceService();
                return (ActionResult)new OkObjectResult(await persistenceService.RetrieveActiveTrips());
            }
            catch (Exception e)
            {
                var error = $"GetActiveTrips failed: {e.Message}";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }
        }

        [FunctionName("GetTrip")]
        public static async Task<IActionResult> GetTrip([HttpTrigger(AuthorizationLevel.Function, "get", Route = "trips/{code}")] HttpRequest req,
            string code,
            ILogger log)
        {
            log.LogInformation("GetTrip triggered....");

            try
            {
                var persistenceService = ServiceFactory.GetPersistenceService();
                return (ActionResult)new OkObjectResult(await persistenceService.RetrieveTrip(code));
            }
            catch (Exception e)
            {
                var error = $"GetTrip failed: {e.Message}";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }
        }

        [FunctionName("CreateTrip")]
        public static async Task<IActionResult> CreateTrip([HttpTrigger(AuthorizationLevel.Function, "post", Route = "trips")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("CreateTrip triggered....");

            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                TripItem trip = JsonConvert.DeserializeObject<TripItem>(requestBody);

                // validate
                if (trip.Passenger == null || string.IsNullOrEmpty(trip.Passenger.Code))
                    throw new Exception("A passenger with a valid code must be attached to the trip!!");

                trip.EndDate = null;
                var persistenceService = ServiceFactory.GetPersistenceService();
                return (ActionResult)new OkObjectResult(await persistenceService.UpsertTrip(trip));
            }
            catch (Exception e)
            {
                var error = $"CreateTrip failed: {e.Message}";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }
        }

        [FunctionName("EVGH_TripExternalizations2SignalR")]
        public static async Task ProcessTripExternalizations2SignalR([EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation($"ProcessTripExternalizations2SignalR triggered....EventGridEvent" +
                            $"\n\tId:{eventGridEvent.Id}" +
                            $"\n\tTopic:{eventGridEvent.Topic}" +
                            $"\n\tSubject:{eventGridEvent.Subject}" +
                            $"\n\tType:{eventGridEvent.EventType}" +
                            $"\n\tData:{eventGridEvent.Data}");

            try
            {
                TripItem trip = JsonConvert.DeserializeObject<TripItem>(eventGridEvent.Data.ToString());

                //TODO: Do something with the trip
                //TODO: We can also do different processing based on the event subject
                //TODO: Event subjects are defined in ServerlessMicroservices.Shared.Helpers.Constants
            }
            catch (Exception e)
            {
                var error = $"ProcessTripExternalizations2SignalR failed: {e.Message}";
                log.LogError(error);
                throw e;
            }
        }

        [FunctionName("EVGH_TripExternalizations2PowerBI")]
        public static async Task ProcessTripExternalizations2PowerBI([EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation($"ProcessTripExternalizations2PowerBI triggered....EventGridEvent" +
                            $"\n\tId:{eventGridEvent.Id}" +
                            $"\n\tTopic:{eventGridEvent.Topic}" +
                            $"\n\tSubject:{eventGridEvent.Subject}" +
                            $"\n\tType:{eventGridEvent.EventType}" +
                            $"\n\tData:{eventGridEvent.Data}");

            try
            {
                TripItem trip = JsonConvert.DeserializeObject<TripItem>(eventGridEvent.Data.ToString());

                //TODO: Do something with the trip
                //TODO: We can also do different processing based on the event subject
                //TODO: Event subjects are defined in ServerlessMicroservices.Shared.Helpers.Constants
            }
            catch (Exception e)
            {
                var error = $"ProcessTripExternalizations2PowerBI failed: {e.Message}";
                log.LogError(error);
                throw e;
            }
        }
    }
}