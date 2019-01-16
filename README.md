# Arduino LiveSerial
Con frecuencia he necesitado en proyectos **obtener una serie de datos enviados por puerto serie**, por ejemplo, desde un microprocesador como Arduino, ESP8266 o ESP32. Sin embargo, no he encontrado un software, gratuito o comercial, que cumpliera lo que necesitaba.

Así surge LiveSerial, un programa que **permite graficar y realizar estadísticas** de los datos obtenidos por puerto serie en tiempo real, y exportar los datos obtenidos a diversos formatos.

Otra ventaja de LiveSerial es que almacena el instante en el que es recibido los valores. Así, las gráficas tienen un eje X temporal (en lugar de la mayoría de valores que muestran los valores "todo seguidos").

LiveSerial está programado en C# + WPF, y está disponible en Windows 7-10. Como sabemos, Windows no es un sistema de tiempo real, y LiveSerial tiene una velocidad máxima de muestreo reducida. No está pensando para hacer, por ejemplo, un osciloscopio. La tasa de refresco que podéis obtener depende de vuestro ordenador, pero esperar un máximo de 20-50 muestras por segundo. 

## Usando LiveSerial
La idea principal de LiveSerial es que sea **mínimamente intrusivo** con el código del microprocesador. No es necesario ninguna librería, ni ningún código complejo.

Tenemos dos modos de funcionamiento:

### Modo normal
Para comunicarnos con LiveSerial únicamente tenemos que enviarle por puerto serie un comando en formato

>ID:value

Siendo:
* ID el nombre de la serie.
* Value el valor numérico a almacenar.

Por ejemplo,

> Temp1:257.8

Cuando LiveSerial recibe un ID nuevo, crea una nueva serie, las añade a las anteriores, y comienza su registro.

### Modo síncrono
En el modo "norma " la fecha y hora del punto es el instante en el que es recibido por el ordenador. Sin embargo, como decimos, Windows no es un sistema de tiempo real por lo que, dependiendo de la carga del SO, puede variaciones de milisegundos desde el envió.

Un ejemplo es que probéis a maximizar o cambiar el tamaño de LiveSerial, o ejecutar otro programa mientas está recibiendo datos. Veréis que la gráfica se deforma, por el retraso que el SO provoca para atender los datos recibidos por puerto serie.

En muchos casos el modo "normal" es suficiente, pero en ciertos casos en los que se requiere mayor precisión es posible enviar la marca temporal desde el microprocesador con el comando.

> ID:value@millis

Siendo,

* ID el nombre de la serie de datos.
* Value, el valor numérico a almacenar.
* Millis, los milisegundos enviados desde el procesador.

LiveSerial recoge los datos y los ajusta, atendiendo a las marcas de tiempo enviados por el microprocesador, para que la escala temporal se ajuste con precisión.

### Mensajes
Los datos recibidos por puerto serie que no contienen el identificador de serie (por defecto ':') son interpretados como "mensajes". Estos se muestran en su propia ventana.

Así mismo es posible enviar mensajes al microprocesador. Estos mensajes son registrados y se muestran en su propia ventana.

### Macros
Si bien no es necesario emplear ninguna librería para usar LiveSerial en el procesador, se facilitan las siguientes macros para hacer más sencillo su uso
```c++
// modo normal
#define LIVESERIAL(id, value) { Serial.print(id);  Serial.print(':');  Serial.println(value); }

// modo sincrono
#define LIVESERIAL_MILLIS(id, value) { unsigned long ms = millis(); Serial.print(id);  Serial.print(':');  Serial.print(value);  Serial.print('@');  Serial.println(ms); }
```
Estas macros están disponibles a modo de recordatorio en LiveSerial pulsando el botón "?".


## Interface de usuario
![Screehshot1](/Screenshots/MainScreen.png)

### Conectar
Para conectar LiveSerial emplear el botón deslizante de la parte superior de la ventana.
Si no aparece ningún puerto seleccionado, abrir la pestaña de opciones para recargar los puertos serie.

### Area de gráficas
Aquí se muestran en una gráfica los N últimos valores recibidos (el número de elementos mostrados se controla desde las opciones, para evitar saturar el programa).

### Area de mensajes
Desde aquí podemos enviar mensajes por puerto serie, así como ver los mensajes enviados y recibidos.

### Area de estadísticas
A medida que se reciben datos de la serie, se calculan en tiempo real sus estadísticas. Estas estadísticas son:
* Name, nombre de la serie
* Count, número total de elementos recibidos
* Value, último valor recibido
* ΔValue, incremento respecto al valor anterior
* Δt, incremento temporal respecto al anterior
* Sum, suma total
* Slope, pendiente de los últimos elementos
* Min, valor mínimo recibido
* Max, valor máximo recibodo
* Range, diferencia entre Max y Min
* MinT, tiempo del primer dato recibido
* MaxT, tiempo del último dato recibido
* Peak, valor del último pico registrado
* PeakT, tiempo del último pico registrado
* Interval, diferencia de tiempo entre dos últimos picos
* Frequency, frecuencia basada en los últimos dos picos

### Ver datos y exportar datos 
![Screehshot3](/Screenshots/ViewSeriesScreen.png)
Podemos visualizar todos los datos recibidos, haciendo click en el icono de la serie. Se abrirá una nueva ventana que muestra los datos de la serie. 

Desde esta ventana podemos elegir exportar a diversos formatos, CSV, JSON, y Excel. En el caso de Excel, la hoja exportada contiene los datos y una gráfica de los valores.

![Screehshot4](/Screenshots/excel-export.png)

### Opciones
![Screehshot2](/Screenshots/Options.png)
Pulsando el botón de opciones accedemos a la ventana para modificarlas. Las opciones se guardan de una ejecución a otra del programa, por usuario.

En primer lugar, tenemos el puerto serie y velocidad de transmisión que queremos emplear.

Por otro lado, tenemos el número de elementos (por serie) a graficar. A medida que la gráfica aumenta de número de puntos la carga de procesado aumenta, especialmente si tenemos muchas series. Por ello, sólo se grafican los N últimos elementos recibidos. 

Por ejemplo, en caso de una única serie valores de 1000-1000 son frecuentes, pero si tenemos varias series puede que tengamos que bajarlo a 100-500, o sobrecargaremos el programa.

Finalmente, podemos cambiar los separadores por defecto ':' y '@' por cualquier otro carácter.

## Ejemplo de uso
Un ejemplo de uso de LiveSerial con Arduino sería el siguiente:

```c++
#define LIVESERIAL(id, value) { Serial.print(id);  Serial.print(':');  Serial.println(value); }
#define LIVESERIAL_MILLIS(id, value) { unsigned long ms = millis(); Serial.print(id);  Serial.print(':');  Serial.print(value);  Serial.print('@');  Serial.println(ms); }

void setup() {
  Serial.begin(115200);
}

void loop() {
    while(Serial.available())
    {
      Serial.print((char)Serial.read());
    }

    LIVESERIAL("NORMAL", 2.0*cos(millis()/500.0));
    LIVESERIAL_MILLIS("WITH_MILLIS", 2.0*cos(ms/500.0));
    delay(100);
}
```

Este ejemplo grafica dos funciones seno, uno con el modo "normal" y otro con el modo "síncrono".

Además, el programa emite todo lo que recibe. De esta forma podéis probar a enviar mensajes, recibirlos, incluso probar a enviar nuevas series en tiempo real (enviando un ID:value)



## TO-DOs:
- [ ] Triggers: Acciones que se realizan cuando el valor de una serie cumple una condición.
- [ ] Scripts: Archivos de comandos que se ejecutan de forma secuencia mediante LiveSerial. 

## Dependencias:
* LiveSerial usa las siguientes librerías:
* RJCP: Puerto serie mejorado para .Net
* Oxyplot: Gráficas en .NET
* MahApps: Estilo visual aplicaciones XAML.
* Material design XAML: Estilo visual "Material design" para XAML.
* ReactiveUI: Framework reactive MVVM para WPF.
* NewtsonJSON: Trabajos con ficheros JSON.
* EEPlus: Trabajo con ficheros EXCEL.




