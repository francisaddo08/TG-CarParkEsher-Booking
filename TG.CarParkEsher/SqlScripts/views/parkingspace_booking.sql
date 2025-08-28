  CREATE VIEW v_parking_booking
AS
SELECT
               BookingId,
	          EmployeeId,
                   ContactId,
                FirstName,
                   LastName,
              ParkingSpace,
                    DayName,
                DateValue,
                parkingspace.parkingspaceid,
                parkingspace.parkingstructureid,
                parkingspace.ev_exclusive,
                parkingspace.bluebadge
           
           FROM v_employee_contact_booking
           INNER JOIN "parkingspace" ON  v_employee_contact_booking.ParkingSpace = parkingspace.parkingspaceid