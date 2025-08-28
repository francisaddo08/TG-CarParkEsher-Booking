  CREATE VIEW v_employee_contact_account
    AS SELECT
	       employee.employeeid AS EmployeeId,
           contact.contactid AS ContactId,
           contact.firstname AS FirstName,
           contact.lastname AS LastName,
           account.vehicletype AS VehicleType,
           account.salt AS Salt,
           account.passwordhash AS PasswordHash,
           account.isactive AS IsActive,
           account.isblocked AS IsBlocked
           FROM "employee" 
           INNER JOIN "contact" ON employee.contact_id = contact.contactid
           INNER JOIN "account" ON account.contact_id = contact.contactid
         