


   
  SELECT bookingid, bookee_id,  parkingspace_id, parkingstructure_id, datevalue, dayname
   FROM booking
   INNER JOIN daysofweek on booking.dayofweek_id = daysofweek.id
   WHERE bookee_id = 1
   ORDER BY  bookingid DESC LIMIT 1
     