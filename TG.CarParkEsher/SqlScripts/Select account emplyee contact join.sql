SELECT
	       employee.employeeid AS EmployeeId,
           contact.contactid AS ContactId,
           contact.firstname AS FirstName,
           contact.lastname AS LastName,
           account.vehicletype AS VehicleType,
           account.salt AS Salt,
           account.passwordhash AS PasswordHash
           FROM "employee" 
           INNER JOIN "contact" ON employee.contact_id = contact.contactid
           INNER JOIN "account" ON account.contact_id = contact.contactid
           WHERE account.isactive = 1  AND account.isblocked = 0