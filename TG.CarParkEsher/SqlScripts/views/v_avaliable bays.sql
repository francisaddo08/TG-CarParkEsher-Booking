  CREATE VIEW v_avaliableparkingspaces AS
      
      SELECT DISTINCT v_parkingSpaceStructure.ParkingSpaceId, v_parkingSpaceStructure.StructureName,
       v_parkingSpaceStructure.EV,  v_parkingSpaceStructure.BlueBagde,
        daysofweek.dayname, daysofweek.datevalue
                      
         FROM v_parkingSpaceStructure
         LEFT JOIN weeklyparkingspace On weeklyparkingspace.parkingspace_id =v_parkingSpaceStructure.ParkingSpaceId
         LEFT JOIN daysofweek ON daysofweek.id =weeklyparkingspace.dayofweek_id
         WHERE weeklyparkingspace.parkingspace_id NOT IN(SELECT ParkingSpace FROM v_parking_booking)