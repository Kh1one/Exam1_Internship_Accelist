using exam1.Entities;
using exam1.models;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace exam1.services
{
    public class AvailableTicketService
    {
        private readonly AccelokaContext _db;
        public AvailableTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<List<AvailableTicketResponseModel>>> GetAvailableTicketData(AvailableTicketRequestModel request)
        {
            //create query & join
            var query = _db.AvailableTickets
            .Join(
                _db.CategoryTickets,
                at => at.CategoryId,
                ct => ct.CategoryId,
                (at, ct) => new { at, ct }
            );

            query = query.Where(joined => joined.at.Quota >= 1);

            //filter
            if (request.TicketCode != string.Empty)
            {
                query = query.Where(joined => joined.at.TicketCode == request.TicketCode);
            }
            if (request.TicketName != string.Empty)
            {
                query = query.Where(joined => joined.at.TicketName == request.TicketName);
            }
            if (request.CategoryName != string.Empty)
            {
                query = query.Where(joined => joined.ct.CategoryName == request.CategoryName);
            }
            if (request.Price != 0)
            {
                query = query.Where(joined => joined.at.Price <= request.Price);
            }
            if (request.MinEventDate != DateTime.MinValue)
            {
                query = query.Where(joined => joined.at.EventDate >= request.MinEventDate);
            }
            if (request.MaxEventDate != DateTime.MinValue)
            {
                query = query.Where(joined => joined.at.EventDate <= request.MaxEventDate);
            }


            //order by and order state
            if (request.OrderBy != string.Empty)
            {
                if (request.OrderState == string.Empty || request.OrderState.ToLower() == "ascending")
                {
                    switch (request.OrderBy)
                    {
                        case "EventDate":
                            query = query.OrderBy(joined => joined.at.EventDate);
                            break;

                        case "Quota":
                            query = query.OrderBy(joined => joined.at.Quota);
                            break;

                        case "TicketCode":
                            query = query.OrderBy(joined => joined.at.TicketCode);
                            break;

                        case "TicketName":
                            query = query.OrderBy(joined => joined.at.TicketName);
                            break;

                        case "CategoryName":
                            query = query.OrderBy(joined => joined.ct.CategoryName);
                            break;

                        case "Price":
                            query = query.OrderBy(joined => joined.at.Price);
                            break;

                        default:

                            return new ServiceResponse<List<AvailableTicketResponseModel>>
                            {
                                //Success = false,
                                Title = "Invalid OrderBy value",
                                Status = 422,
                                Detail = $"OrderBy value of '{request.OrderBy}' is invalid, allowed values are " +
                                "'EventDate', 'Quota', 'TicketCode', 'TicketName', 'CategoryName', or 'Price'."
                            };
                    }
                }
                else if (request.OrderState.ToLower() == "descending")
                {
                    switch (request.OrderBy)
                    {
                        case "EventDate":
                            query = query.OrderByDescending(joined => joined.at.EventDate);
                            break;

                        case "Quota":
                            query = query.OrderByDescending(joined => joined.at.Quota);
                            break;

                        case "TicketCode":
                            query = query.OrderByDescending(joined => joined.at.TicketCode);
                            break;

                        case "TicketName":
                            query = query.OrderByDescending(joined => joined.at.TicketName);
                            break;

                        case "CategoryName":
                            query = query.OrderByDescending(joined => joined.ct.CategoryName);
                            break;

                        case "Price":
                            query = query.OrderByDescending(joined => joined.at.Price);
                            break;

                        default:
                            return new ServiceResponse<List<AvailableTicketResponseModel>>
                            {
                                //Success = false,
                                Title = "Invalid OrderBy value",
                                Status = 422,
                                Detail = $"OrderBy value of '{request.OrderBy}' is invalid, allowed values are " +
                                        "'EventDate', 'Quota', 'TicketCode', 'TicketName', 'CategoryName', or 'Price'."
                            };
                            //throw new ArgumentException($"Invalid OrderBy value: {request.OrderBy}. " +
                            //    $"Allowed values are 'EventDate', 'Quota', 'TicketCode', 'TicketName', 'CategoryName', or 'Price'.");

                    }
                }
                else
                {
                    return new ServiceResponse<List<AvailableTicketResponseModel>>
                    {
                        //Success = false,
                        Title = "Invalid OrderState value",
                        Status = 422,
                        Detail = $"OrderState value of '{request.OrderState}' is invalid, allowed values are " +
                                 "'Ascending' or 'Descending'."
                    };
                }
            }

            if (request.OrderBy == string.Empty)
            {
                if (request.OrderState.ToLower() == "ascending" || request.OrderState == string.Empty){
                    query = query.OrderBy(joined => joined.at.TicketCode);
                }
                else if (request.OrderState.ToLower() == "descending")
                {
                    query = query.OrderByDescending(joined => joined.at.TicketCode);
                }
                else
                {
                    return new ServiceResponse<List<AvailableTicketResponseModel>>
                    {
                        //Success = false,
                        Title = "Invalid OrderState value",
                        Status = 422,
                        Detail = $"OrderState value of '{request.OrderState}' is invalid, allowed values are " +
                                 "'Ascending' or 'Descending'."
                    };
                }

            }
            
            

            //get data
            var data = await query.Select(Q => new AvailableTicketResponseModel {
                EventDate = Q.at.EventDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TicketCode = Q.at.TicketCode,
                TicketName = Q.at.TicketName,
                Quota = Q.at.Quota,
                Price = Q.at.Price,
                CategoryName = Q.ct.CategoryName
            }).ToListAsync();


            return new ServiceResponse<List<AvailableTicketResponseModel>>
            {
                Data = data
            };

        }
    }
}
