   
    CREATE VIEW v_employee_contact 
    AS SELECT
	       employee.employeeid AS EmployeeId,
           contact.contactid AS ContactId,
           contact.firstname AS FirstName,
           contact.lastname AS LastName FROM "employee" INNER JOIN "contact" ON employee.contact_id = contact.contactid