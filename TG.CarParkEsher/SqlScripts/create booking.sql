

CREATE TABLE booking (bookingid INTEGER PRIMARY KEY,
       bookee_id INTEGER REFERENCES bookee (bookeeid), 
       dateofbooking DATE NOT NULL,
       dayofweek_id INTEGER REFERENCES daysofweek(id),
       parkingspace_id INTEGER, 
       parkingstructure_id INTEGER, 
       FOREIGN KEY (parkingspace_id, parkingstructure_id) 
       REFERENCES parkingspace (parkingspaceid, parkingstructureid));