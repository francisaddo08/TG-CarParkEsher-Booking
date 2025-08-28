DROP TABLE IF EXISTS "weeklyspace";

CREATE TABLE weeklyparkingspace(
  parkingspaceid INTEGER NOT NULL, 
  dayofweekid INTEGER NOT NULL, 
  
  FOREIGN KEY(parkingspaceid) REFERENCES parkingspace(parkingspaceid),
  FOREIGN KEY(dayofweekid) REFERENCES daysofweek(id)
);
