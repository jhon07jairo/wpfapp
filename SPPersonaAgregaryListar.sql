CREATE PROCEDURE dbo.AgregarPersona
    @Nombre NVARCHAR(50),
    @Apellidos NVARCHAR(50) = NULL,
    @Documento INT,
    @FechaNacimiento DATE,
    @Sexo CHAR(1),
    @CodigoPais VARCHAR(10)
AS
BEGIN
    -- Verificar si el país existe
    IF EXISTS (SELECT 1 FROM Paises WHERE CodigoPais = @CodigoPais)
    BEGIN
        -- Insertar la persona en la tabla
        INSERT INTO Personas (Nombre, Apellidos, Documento, FechaNacimiento, Sexo, CodigoPais)
        VALUES (@Nombre, @Apellidos, @Documento, @FechaNacimiento, @Sexo, @CodigoPais);

        -- Mensaje de confirmación
        PRINT 'Persona agregada exitosamente.';
    END
    ELSE
    BEGIN
        -- Mensaje de error si el país no existe
        PRINT 'Error: El código de país no existe.';
    END
END;

CREATE PROCEDURE dbo.ListarPersonas
AS
BEGIN
    SELECT 
        p.Id,
        p.Nombre,
        p.Apellidos,
        p.Documento,
        p.FechaNacimiento,
        p.Sexo,
        p.CodigoPais,
        pa.NombrePais
    FROM 
        Personas p
    INNER JOIN 
        Paises pa ON p.CodigoPais = pa.CodigoPais
    ORDER BY 
        p.Nombre, p.Apellidos;
END;

