CREATE VIEW v_employee_contact_booking
AS
SELECT
           booking.bookingid AS BookingId,
	       employee.employeeid AS EmployeeId,
           contact.contactid AS ContactId,
           contact.firstname AS FirstName,
           contact.lastname AS LastName,
           booking.parkingspace_id AS ParkingSpace,
           daysofweek.dayname AS DayName,
           daysofweek.datevalue AS DateValue
           
           FROM "employee" 
           INNER JOIN "contact" ON employee.contact_id = contact.contactid
           INNER JOIN "booking" ON booking.bookee_id = contact.contactid
           INNER JOIN  "daysofweek" ON booking.dayofweek_id = daysofweek.daynumber;