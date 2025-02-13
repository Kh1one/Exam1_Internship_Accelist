using Azure.Core;
using exam1.Entities;
using exam1.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Net.Sockets;
using System.Xml;

namespace exam1.services
{
    public class BookedTicketService
    {
        private readonly AccelokaContext _db;
        public BookedTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<BookedTicketDetailSummaryResponseModel>> InsertNewBookedTicket(List<BookedTicketDetailRequestModel> request)
        {
            bool errorExists = false;//to keep track of an error, and used later to throw exception
            //to give each error its own exception line
            string tempTitle = string.Empty;
            int tempStatusCode = 0;
            string tempDetail = string.Empty;

            //add new id in parent table
            //kalau insert detailnya error, nnt ini dihapus
            var newBookedTicketId = new BookedTicket();

            _db.BookedTickets.Add(newBookedTicketId);
            await _db.SaveChangesAsync();

            //check each item to make sure no errors
            foreach (var item in request)
            {
                var existingData = await _db.AvailableTickets
                .Where(Q => Q.TicketCode == item.TicketCode)
                .FirstOrDefaultAsync();

                if (existingData == null)
                {
                    tempTitle = "Invalid TicketCode";
                    tempStatusCode = 422;
                    tempDetail = $"TicketCode value of '{item.TicketCode}' does not exist in database";
                    //argumentExceptionString = $"Invalid TicketCode value: {item.TicketCode}. Value not found in database.";
                    errorExists = true;
                    break;
                }
                else if (existingData.Quota == 0)
                {
                    tempTitle = "Available quota is 0";
                    tempStatusCode = 403;
                    tempDetail = $"Available quota for TicketCode: {item.TicketCode} is 0";
                    //argumentExceptionString = $"Quota for TicketCode: {item.TicketCode} is 0.";
                    errorExists = true;
                    break;
                }
                else if (existingData.Quota < item.Quantity || item.Quantity == 0)
                {
                    tempTitle = "Wrong item quantity value";
                    tempStatusCode = 403;
                    tempDetail = $"Provided quantity of {item.Quantity} may be 0 or over available quota for TicketCode which is {item.TicketCode}";
                    //argumentExceptionString = $"Invalid item Quantity value: {item.Quantity}. Incorrect value, quantity may be over quantity or 0.";
                    errorExists = true;
                    break;
                }
                else if (existingData.EventDate <= DateTime.Now)
                {
                    tempTitle = "Invalid date";
                    tempStatusCode = 403;
                    tempDetail = $"Event's dateTime value of '{existingData.EventDate}' may have already passed or is equal to current date";
                    //argumentExceptionString = $"Invalid dateTime value: {existingData.EventDate}. Event date may have passed or is equal to current date.";
                    errorExists = true;
                    break;
                }
            }

            if (errorExists)
            {
                _db.BookedTickets.Remove(newBookedTicketId);
                await _db.SaveChangesAsync();

                return new ServiceResponse<BookedTicketDetailSummaryResponseModel>
                {
                    Title = tempTitle,
                    Status = tempStatusCode,
                    Detail = tempDetail
                };
                //throw new ArgumentException(argumentExceptionString);
            }

            //insert each request data
            foreach (var item in request)
            {
                //update each availableTicket's quota
                var availableTicket = await _db.AvailableTickets
               .Where(Q => Q.TicketCode == item.TicketCode)
               .FirstOrDefaultAsync();

                availableTicket.Quota -= item.Quantity;

                _db.AvailableTickets.Update(availableTicket);
                await _db.SaveChangesAsync();


                //add new data to bookedTicketDetail
                var addItem = new BookedTicketDetail
                {
                    TicketCode = item.TicketCode,
                    Quantity = item.Quantity,
                    BookedTicketDetailId = newBookedTicketId.BookedTicketId
                };
                _db.BookedTicketDetails.Add(addItem);
                await _db.SaveChangesAsync();

            }

            //build response model
            var result = await GetBookedTicketDetailModels(newBookedTicketId.BookedTicketId);


            // Group data by category and construct result
            var groupedResult = result
                .GroupBy(r => r.CategoryName)
                .Select(group => new BookedTicketDetailCategoryResponseModel
                {
                    CategoryName = group.Key,
                    SummaryPrice = group.Sum(ticket => ticket.Price * ticket.Quantity),
                    Tickets = group.Select(ticket => new BookedTicketDetailResponseModel
                    {
                        TicketCode = ticket.TicketCode,
                        TicketName = ticket.TicketName,
                        Price = ticket.Price
                    }).ToList()
                }).ToList();

            // Construct final output
            var finalOutput = new BookedTicketDetailSummaryResponseModel
            {
                PriceSummary = groupedResult.Sum(g => g.SummaryPrice),
                TicketsPerCategory = groupedResult
            };


            return new ServiceResponse<BookedTicketDetailSummaryResponseModel>
            {
                Data = finalOutput
            };
        }

        public async Task<ServiceResponse<List<GetBookedTicketCategoryModel>>> GetBookedTicketDetails(int BookedTicketId)
        {
            var result = await GetBookedTicketDetailModels(BookedTicketId);


            if (result.Any())
            {
                // Group data by category and construct the result
                var groupedResult = result
                    .GroupBy(c => c.CategoryName)
                    .Select(group => new GetBookedTicketCategoryModel
                    {
                        CategoryName = group.Key,
                        QuantityPerCategory = group.Sum(ticket => ticket.Quantity),
                        Tickets = group.Select(ticket => new GetBookedTicketModel
                        {
                            TicketCode = ticket.TicketCode,
                            TicketName = ticket.TicketName,
                            EventTime = ticket.EventDate.ToString("yyyy-MM-dd HH:mm:ss")
                        }).ToList()
                    }).ToList();

                return new ServiceResponse<List<GetBookedTicketCategoryModel>>
                {
                    Data = groupedResult
                };

            }

            return new ServiceResponse<List<GetBookedTicketCategoryModel>>
            {
                Title = "BookedTicketId not found",
                Status = 422,
                Detail = $"BookedTicketId of: {BookedTicketId} does not exist in database"
            };
            //throw new ArgumentException($"BookedTicketId of: {BookedTicketId} not found in database");
        }

        public async Task<ServiceResponse<List<UpdatedBookedTicketModel>>> DeleteBookedTicket(int RequestBookedTicketId, string RequestTicketCode, int RequestQuantity)
        {
            var existingData = await _db.BookedTicketDetails
                .Where(Q => Q.BookedTicketDetailId == RequestBookedTicketId)
                .ToListAsync();

            //validate null in list of ticket data
            if (!existingData.Any())
            {
                return new ServiceResponse<List<UpdatedBookedTicketModel>>
                {
                    Title = "BookedTicketId not found",
                    Status = 422,
                    Detail = $"BookedTicketId of: {RequestBookedTicketId} does not exist in database"
                };
                //throw new ArgumentException($"BookedTicketId of: {RequestBookedTicketId} not found in database");
            }

            //look for bookedticket with the same TicketCode
            var ticketData = existingData
                .Where(Q => Q.TicketCode == RequestTicketCode)
                .FirstOrDefault();

            //validate null and quantity in ticket
            if (ticketData == null)
            {
                return new ServiceResponse<List<UpdatedBookedTicketModel>>
                {
                    Title = "TicketCode not found",
                    Status = 422,
                    Detail = $"TicketCode of: {RequestTicketCode} does not exist in database"
                };
                //throw new ArgumentException($"TicketCode of: {RequestTicketCode} not found in database");
            }
            if (ticketData.Quantity < RequestQuantity)
            {
                return new ServiceResponse<List<UpdatedBookedTicketModel>>
                {
                    Title = "Wrong RequestQuantity value",
                    Status = 403,
                    Detail = $"Provided quantity of: {RequestQuantity} is over the quantity of previously booked ticket in database, which is {ticketData.Quantity}"
                };
                //throw new ArgumentException($"Quantity of: {RequestQuantity} over quantity of booked ticket in database");
            }

            //remove entry if remaining quantity is 0
            if (ticketData.Quantity == RequestQuantity)
            {
                _db.BookedTicketDetails.Remove(ticketData);
                await _db.SaveChangesAsync();


                //remove parent entry if all child entries are removed
                bool hasChildEntries = await _db.BookedTicketDetails
                    .AnyAsync(Q => Q.BookedTicketDetailId == RequestBookedTicketId);

                if (!hasChildEntries)
                {
                    var existingBookedTicketsEntry = await _db.BookedTickets
                        .Where(Q => Q.BookedTicketId == RequestBookedTicketId)
                        .FirstOrDefaultAsync();

                    _db.BookedTickets.Remove(existingBookedTicketsEntry);
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                //update quantity in database
                ticketData.Quantity = ticketData.Quantity - RequestQuantity;
                _db.BookedTicketDetails.Update(ticketData);
                await _db.SaveChangesAsync();
            }

            //get data for output
            var result = await GetBookedTicketDetailModels(RequestBookedTicketId);

            //build output
            var finalResult = result
            .Select(res => new UpdatedBookedTicketModel
            {
                TicketCode = res.TicketCode,
                TicketName = res.TicketName,
                Quantity = res.Quantity,
                CategoryName = res.CategoryName
            }).ToList();

            return new ServiceResponse<List<UpdatedBookedTicketModel>>
            {
                Data = finalResult
            };
        }

        public async Task<ServiceResponse<List<UpdatedBookedTicketModel>>> UpdateBookedTicketDetails(List<UpdateBookedTicketRequestModel> request, int RequestBookedTicketId)
        {
            var existingData = await _db.BookedTicketDetails
                .Where(Q => Q.BookedTicketDetailId == RequestBookedTicketId)
                .ToListAsync();

            if (!existingData.Any())
            {
                return new ServiceResponse<List<UpdatedBookedTicketModel>>
                {
                    Title = "BookedTicketId not found",
                    Status = 422,
                    Detail = $"BookedTicketId of: {RequestBookedTicketId} does not exist in database"
                };
                //throw new ArgumentException($"BookedTicketId of: {RequestBookedTicketId} not found in database");
            }

            //validate 
            foreach (var item in request)
            {
                //look for bookedticket with the same TicketCode
                var ticketData = existingData
                    .Where(Q => Q.TicketCode == item.TicketCode)
                    .FirstOrDefault();

                if(ticketData == null)
                {
                    return new ServiceResponse<List<UpdatedBookedTicketModel>>
                    {
                        Title = "TicketCode not found",
                        Status = 422,
                        Detail = $"TicketCode of: {item.TicketCode} does not exist in database"
                    };
                    //throw new ArgumentException($"TicketCode of: {item.TicketCode} not found in database");
                }

                //validate quantity against available quota 
                var existingAvailableTicketsEntry = await _db.AvailableTickets
                    .Select(Q => new UpdateBookedTicketRequestModel
                    {
                        TicketCode = item.TicketCode,
                        Quantity = Q.Quota
                    })
                    .Where(Q => Q.TicketCode == item.TicketCode)
                    .FirstOrDefaultAsync();

                if (item.Quantity < 1 ||  item.Quantity > existingAvailableTicketsEntry.Quantity)
                {
                    return new ServiceResponse<List<UpdatedBookedTicketModel>>
                    {
                        Title = "Wrong requested quantity value",
                        Status = 403,
                        Detail = $"Provided quantity of: {item.Quantity} may be less than 1, or is over the quantity available quota in database, which is {existingAvailableTicketsEntry.Quantity}"
                    };
                    //throw new ArgumentException($"Quantity of: {item.Quantity} over quota in database or is less than 1");
                }
            }

            //update data
            foreach (var item in request)
            {
                //ticket data is data in the database
                var ticketData = existingData
                    .Where(Q => Q.TicketCode == item.TicketCode)
                    .FirstOrDefault();

                //find available ticket entry to update the quota
                var existingAvailableTicketsEntry = await _db.AvailableTickets
                    .Where(Q => Q.TicketCode == item.TicketCode)
                    .FirstOrDefaultAsync();

                if (ticketData.Quantity > item.Quantity)
                {
                    existingAvailableTicketsEntry.Quota += (ticketData.Quantity - item.Quantity);
                }
                else
                {
                    existingAvailableTicketsEntry.Quota -= (item.Quantity - ticketData.Quantity);
                }


                //update BookedTicketDetails table
                ticketData.Quantity = item.Quantity;
                _db.BookedTicketDetails.Update(ticketData);
                await _db.SaveChangesAsync();

                //update AvailableTicket table
                _db.AvailableTickets.Update(existingAvailableTicketsEntry);
                await _db.SaveChangesAsync();
            }


            //get data for output
            var result = await GetBookedTicketDetailModels(RequestBookedTicketId);

            //build output
            var finalResult = result
            .Select(res => new UpdatedBookedTicketModel
            {
                TicketCode = res.TicketCode,
                TicketName = res.TicketName,
                Quantity = res.Quantity,
                CategoryName = res.CategoryName
            }).ToList();

            return new ServiceResponse<List<UpdatedBookedTicketModel>>
            {
                Data = finalResult
            };
        }


        public async Task<List<BookedTicketDetailModel>> GetBookedTicketDetailModels(int BookedTicketId)
        {
            var result = await (from bt in _db.BookedTickets
                                join btd in _db.BookedTicketDetails on bt.BookedTicketId equals btd.BookedTicketDetailId
                                join at in _db.AvailableTickets on btd.TicketCode equals at.TicketCode
                                join ct in _db.CategoryTickets on at.CategoryId equals ct.CategoryId
                                where btd.BookedTicketDetailId == BookedTicketId
                                select new BookedTicketDetailModel
                                {
                                    CategoryId = ct.CategoryId,
                                    TicketCode = btd.TicketCode,
                                    Quantity = btd.Quantity,
                                    TicketName = at.TicketName,
                                    Price = at.Price,
                                    CategoryName = ct.CategoryName,
                                    EventDate = at.EventDate
                                }).ToListAsync();
            return result;

        }
    }
}