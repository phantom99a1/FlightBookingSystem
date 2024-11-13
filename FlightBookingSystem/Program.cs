using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightBookingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Example usage
            var bookingSystem = new FlightBookingManager();

            // Add some flights
            bookingSystem.AddFlight("FL001", "New York", "London", new DateTime(2024, 6, 15), 500.00m, 100);
            bookingSystem.AddFlight("FL002", "London", "Paris", new DateTime(2024, 6, 16), 200.00m, 150);

            // Try to book some flights
            var booking1 = bookingSystem.BookFlight("FL001", 2, new string[] { "John Doe", "Jane Doe" });
            var booking2 = bookingSystem.BookFlight("FL002", 1, new string[] { "Alice Smith" });

            // Display bookings
            bookingSystem.DisplayAllBookings();

            // Try to cancel a booking
            bookingSystem.CancelBooking(booking1.BookingId);

            Console.WriteLine("\nAfter cancellation:");
            bookingSystem.DisplayAllBookings();
        }
    }

    public class FlightBookingManager
    {
        private Dictionary<string, Flight> flights = new Dictionary<string, Flight>();
        private List<Booking> bookings = new List<Booking>();
        private int nextBookingId = 1;

        public void AddFlight(string flightNumber, string origin, string destination, DateTime departureTime, decimal price, int capacity)
        {
            //Check for the null fightNumber
            if (string.IsNullOrEmpty(flightNumber))
            {
                throw new ArgumentException("Flight Number isn't null!");
            }
            if (flights.ContainsKey(flightNumber))
            {
                throw new ArgumentException("Flight already exists");
            }

            var flight = new Flight
            {
                FlightNumber = flightNumber,
                Origin = origin,
                Destination = destination,
                DepartureTime = departureTime,
                Price = price,
                Capacity = capacity,
                AvailableSeats = capacity
            };

            flights.Add(flightNumber, flight); // Bug 1: Doesn't check for null flightNumber
        }

        public Booking BookFlight(string flightNumber, int numberOfPassengers, string[] passengerNames)
        {
            if (!flights.ContainsKey(flightNumber))
            {
                throw new ArgumentException("Flight not found");
            }

            var flight = flights[flightNumber];

            // Bug 2: Incorrect logic for checking available seats
            if (flight.AvailableSeats < numberOfPassengers)
            {
                numberOfPassengers = flight.AvailableSeats; // Bug 3: Decrements seats even when booking should fail
                throw new InvalidOperationException("Not enough seats available");
            }

            // Bug 4: Doesn't verify that passengerNames length matches numberOfPassengers

            var booking = new Booking
            {
                BookingId = nextBookingId++,
                FlightNumber = flightNumber,
                PassengerNames = passengerNames,
                TotalPrice = CalculateTotalPrice(flight, numberOfPassengers)
            };

            bookings.Add(booking);
            return booking;
        }

        private decimal CalculateTotalPrice(Flight flight, int numberOfPassengers)
        {
            // Bug 5: Incorrect price calculation
            return flight.Price * (numberOfPassengers - 1);
        }

        public void CancelBooking(int bookingId)
        {
            // Bug 6: Doesn't update available seats when canceling
            bookings.RemoveAll(b => b.BookingId == bookingId);
        }

        public void DisplayAllBookings()
        {
            foreach (var booking in bookings)
            {
                // Bug 7: Potential null reference exception if flight doesn't exist
                var flight = flights[booking.FlightNumber];
                Console.WriteLine($"Booking ID: {booking.BookingId}");
                Console.WriteLine($"Flight: {flight.Origin} to {flight.Destination}");
                Console.WriteLine($"Passengers: {string.Join(", ", booking.PassengerNames)}");
                Console.WriteLine($"Total Price: ${booking.TotalPrice}");
                Console.WriteLine();
            }
        }

        // Bug 8: Missing method to update flight details

        // Bug 9: No validation for past dates when adding flights
    }

    public class Flight
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class Booking
    {
        public int BookingId { get; set; }
        public string FlightNumber { get; set; }
        public string[] PassengerNames { get; set; }
        public decimal TotalPrice { get; set; }
    }
}