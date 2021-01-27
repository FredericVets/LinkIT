DECLARE @fullAccess varchar(256) = 'read, create, modify, delete'
DECLARE @readModifyAccess varchar(256) = 'read, modify'

-- Full access
	-- Inge
	INSERT INTO UserRole VALUES ('u0032885', @fullAccess)
	-- Frederic
	INSERT INTO UserRole VALUES ('u0122713', @fullAccess)
	-- Martijn
	INSERT INTO UserRole VALUES ('u0049806', @fullAccess)
	-- Davy
	INSERT INTO UserRole VALUES ('u0058592', @fullAccess)

-- readModifyAccess
	-- Vincent
	INSERT INTO UserRole VALUES ('u0085622', @readModifyAccess)
	-- Luc
	INSERT INTO UserRole VALUES ('u0001095', @readModifyAccess)
	-- Griet
	INSERT INTO UserRole VALUES ('u0083098', @readModifyAccess)
	-- Grzegorz
	INSERT INTO UserRole VALUES ('u0060137', @readModifyAccess)
	-- Peter
	INSERT INTO UserRole VALUES ('u0053343', @readModifyAccess)
	-- Danny
	INSERT INTO UserRole VALUES ('u0022977', @readModifyAccess)
	-- Herman
	INSERT INTO UserRole VALUES ('u0121444', @readModifyAccess)
	-- Bart
	INSERT INTO UserRole VALUES ('u0028122', @readModifyAccess)
	-- Koen
	INSERT INTO UserRole VALUES ('u0090056', @readModifyAccess)
	-- Sebastiaan
	INSERT INTO UserRole VALUES ('u0064428', @readModifyAccess)
	-- Wouter
	INSERT INTO UserRole VALUES ('u0106341', @readModifyAccess)
	-- Sofie
	INSERT INTO UserRole VALUES ('u0137227', @readModifyAccess)
	-- Robin
	INSERT INTO UserRole VALUES ('u0076411', @readModifyAccess)
	-- Tim
	INSERT INTO UserRole VALUES ('u0126500', @readModifyAccess)