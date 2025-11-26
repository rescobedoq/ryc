using UnityEngine;

[System.Serializable]
public class TwoOptionQuestion
{
    [TextArea]
    public string question;
    public string leftAnswer;
    public string rightAnswer;
    public bool isLeftCorrect;
}

public static class QuestionsBank
{
    // ============================
    // 30 preguntas para Portal 1
    // ============================
    public static readonly TwoOptionQuestion[] Portal1 = new TwoOptionQuestion[]
    {
        new TwoOptionQuestion { question="2 + 3 = ?", leftAnswer="4", rightAnswer="5", isLeftCorrect=false },
        new TwoOptionQuestion { question="5 - 2 = ?", leftAnswer="3", rightAnswer="4", isLeftCorrect=true },
        new TwoOptionQuestion { question="Número mayor", leftAnswer="7", rightAnswer="9", isLeftCorrect=false },
        new TwoOptionQuestion { question="Número menor", leftAnswer="2", rightAnswer="5", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Par? 6", leftAnswer="Par", rightAnswer="Impar", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Impar? 9", leftAnswer="Par", rightAnswer="Impar", isLeftCorrect=false },
        new TwoOptionQuestion { question="Color del cielo", leftAnswer="Azul", rightAnswer="Rojo", isLeftCorrect=true },
        new TwoOptionQuestion { question="Color de la hierba", leftAnswer="Verde", rightAnswer="Morado", isLeftCorrect=true },
        new TwoOptionQuestion { question="Animal que ladra", leftAnswer="Perro", rightAnswer="Gato", isLeftCorrect=true },
        new TwoOptionQuestion { question="Animal que maúlla", leftAnswer="Perro", rightAnswer="Gato", isLeftCorrect=false },
        new TwoOptionQuestion { question="Día después del lunes", leftAnswer="Martes", rightAnswer="Domingo", isLeftCorrect=true },
        new TwoOptionQuestion { question="Día antes del domingo", leftAnswer="Sábado", rightAnswer="Lunes", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Más grande?", leftAnswer="Elefante", rightAnswer="Ratón", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Más pequeño?", leftAnswer="Ballena", rightAnswer="Pez", isLeftCorrect=false },
        new TwoOptionQuestion { question="¿Fruta?", leftAnswer="Manzana", rightAnswer="Pan", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Verdura?", leftAnswer="Tomate", rightAnswer="Lápiz", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Transporte?", leftAnswer="Carro", rightAnswer="Silla", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Del mar?", leftAnswer="Tiburón", rightAnswer="Camello", isLeftCorrect=true },
        new TwoOptionQuestion { question="3 + 4 = ?", leftAnswer="7", rightAnswer="9", isLeftCorrect=true },
        new TwoOptionQuestion { question="10 - 6 = ?", leftAnswer="3", rightAnswer="4", isLeftCorrect=false },
        new TwoOptionQuestion { question="Mitad de 8", leftAnswer="2", rightAnswer="4", isLeftCorrect=false },
        new TwoOptionQuestion { question="Mitad de 10", leftAnswer="5", rightAnswer="8", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Más frío?", leftAnswer="Hielo", rightAnswer="Fuego", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Más caliente?", leftAnswer="Horno", rightAnswer="Refrigeradora", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Insecto?", leftAnswer="Hormiga", rightAnswer="Tortuga", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Vuela?", leftAnswer="Águila", rightAnswer="Caballo", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Para escribir?", leftAnswer="Lápiz", rightAnswer="Cuchara", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Se come?", leftAnswer="Zapato", rightAnswer="Queso", isLeftCorrect=false },
        new TwoOptionQuestion { question="Mes con 28 días", leftAnswer="Febrero", rightAnswer="Agosto", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Estación fría?", leftAnswer="Verano", rightAnswer="Invierno", isLeftCorrect=false },
    };

    // ============================
    // 30 preguntas para Portal 2
    // ============================
    public static readonly TwoOptionQuestion[] Portal2 = new TwoOptionQuestion[]
    {
        new TwoOptionQuestion { question="1 + 7 = ?", leftAnswer="8", rightAnswer="9", isLeftCorrect=true },
        new TwoOptionQuestion { question="9 - 3 = ?", leftAnswer="5", rightAnswer="6", isLeftCorrect=false },
        new TwoOptionQuestion { question="Número par", leftAnswer="5", rightAnswer="8", isLeftCorrect=false },
        new TwoOptionQuestion { question="Número impar", leftAnswer="4", rightAnswer="7", isLeftCorrect=false },
        new TwoOptionQuestion { question="¿Planeta?", leftAnswer="Marte", rightAnswer="Luna", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Satélite?", leftAnswer="Sol", rightAnswer="Luna", isLeftCorrect=false },
        new TwoOptionQuestion { question="Avanzar semáforo", leftAnswer="Rojo", rightAnswer="Verde", isLeftCorrect=false },
        new TwoOptionQuestion { question="Parar semáforo", leftAnswer="Rojo", rightAnswer="Verde", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Mamífero?", leftAnswer="Delfín", rightAnswer="Pez globo", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Selva?", leftAnswer="León", rightAnswer="Pingüino", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Útil en clase?", leftAnswer="Cuaderno", rightAnswer="Televisor", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Para cortar?", leftAnswer="Tijeras", rightAnswer="Regla", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Tiene ruedas?", leftAnswer="Mesa", rightAnswer="Bicicleta", isLeftCorrect=false },
        new TwoOptionQuestion { question="¿Para beber?", leftAnswer="Vaso", rightAnswer="Zapato", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿En el pie?", leftAnswer="Guante", rightAnswer="Zapato", isLeftCorrect=false },
        new TwoOptionQuestion { question="¿Tiene teclado?", leftAnswer="Computadora", rightAnswer="Jarrón", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Para leer?", leftAnswer="Libro", rightAnswer="Pelota", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Se infla?", leftAnswer="Globo", rightAnswer="Mesa", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Cielo de noche?", leftAnswer="Estrella", rightAnswer="Flor", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Hora?", leftAnswer="Reloj", rightAnswer="Cuchillo", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Deporte?", leftAnswer="Fútbol", rightAnswer="Dormir", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿En cocina?", leftAnswer="Sartén", rightAnswer="Auto", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Música?", leftAnswer="Audífonos", rightAnswer="Martillo", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Frío?", leftAnswer="Helado", rightAnswer="Sopa", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Nadar?", leftAnswer="Gafas buceo", rightAnswer="Guantes lana", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Desierto?", leftAnswer="Camello", rightAnswer="Foca", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Polo Norte?", leftAnswer="Oso polar", rightAnswer="Leopardo", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Dientes?", leftAnswer="Cepillo dental", rightAnswer="Peine", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Iluminar?", leftAnswer="Linterna", rightAnswer="Pelota", isLeftCorrect=true },
        new TwoOptionQuestion { question="¿Lluvia?", leftAnswer="Paraguas", rightAnswer="Toalla", isLeftCorrect=true },
    };
}
