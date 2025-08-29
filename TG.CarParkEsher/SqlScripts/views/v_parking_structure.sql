CREATE VIEW v_parkingSpaceStructure AS
         SELECT parkingspace.parkingspaceid, parkingstructure.structurename,
         parkingspace.ev_exclusive AS EV, parkingspace.bluebadge  AS BlueBagde
         FROM parkingspace 
         INNER JOIN parkingstructure ON parkingstructureid = parkingspace.parkingstructureid  