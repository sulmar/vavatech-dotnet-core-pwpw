
# .NET Core

## Przydatne komendy CLI
- ``` dotnet --list-sdks ``` - wyświetlenie listy zainstalowanych SDK
- ``` dotnet new globaljson ``` - utworzenie pliku global.json
- ``` dotnet new globaljson --sdk-version {version} ``` - utworzenie pliku global.json i ustawienie wersji SDK
- ``` dotnet new {template} ``` - utworzenie nowego projektu na podstawie wybranego szablonu
- ``` dotnet new {template} -o {output} ``` - utworzenie nowego projektu w podanym katalogu
- ``` dotnet restore ``` - pobranie bibliotek nuget na podstawie pliku projektu
- ``` dotnet build ``` - kompilacja projektu
- ``` dotnet run ``` - uruchomienie projektu
- ``` dotnet run {app.dll}``` - uruchomienie aplikacji
- ``` dotnet test ``` - uruchomienie testów jednostkowych
- ``` dotnet run watch``` - uruchomienie projektu w trybie śledzenia zmian
- ``` dotnet test ``` - uruchomienie testów jednostkowych w trybie śledzenia zmian
- ``` dotnet add {project.csproj} reference {library.csproj} ``` - dodanie odwołania do biblioteki
- ``` dotnet remove {project.csproj} reference {library.csproj} ``` - usunięcie odwołania do biblioteki
- ``` dotnet new sln ``` - utworzenie nowego rozwiązania
- ``` dotnet sln {solution.sln} add {project.csproj}``` - dodanie projektu do rozwiązania
- ``` dotnet sln {solution.sln} remove {project.csproj}``` - usunięcie projektu z rozwiązania
- ``` dotnet publish -c Release -r {platform}``` - publikacja aplikacji
- ``` dotnet publish -c Release -r win10-x64``` - publikacja aplikacji dla Windows
- ``` dotnet publish -c Release -r linux-x64``` - publikacja aplikacji dla Linux
- ``` dotnet publish -c Release -r osx-x64``` - publikacja aplikacji dla MacOS
- ``` dotnet add package {package-name} ``` - dodanie pakietu nuget do projektu
- ``` dotnet remove package {package-name} ``` - usunięcie pakietu nuget do projektu

## Konfiguracja

- Utworzenie klasy opcji
~~~ csharp
public class CustomerOptions
{
    public int Quantity { get; set; }
}
~~~


- Plik konfiguracyjny appsettings.json

~~~ json
{
  "CustomersModule": {
    "Quantity": 40
  },
  
  ~~~

- Instalacja biblioteki

~~~ bash
 dotnet add package Microsoft.Extensions.Options
~~~

- Użycie opcji

~~~ csharp

public class FakeCustomersService
{
   private readonly CustomerOptions options;

    public FakeCustomersService(IOptions<CustomerOptions> options)
    {
        this.options = options.Value;
    }
}
       
~~~

- Konfiguracja opcji

~~~ csharp
public class Startup
    {
        public IConfiguration Configuration { get; }
    
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddXmlFile("appsettings.xml", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();

        }
        
      public void ConfigureServices(IServiceCollection services)
      {
          services.Configure<CustomerOptions>(Configuration.GetSection("CustomersModule"));
      }
    }
~~~

- Konfiguracja bez interfejsu IOptions<T>
  
~~~ csharp
  public void ConfigureServices(IServiceCollection services)
        {
            var customerOptions = new CustomerOptions();
            Configuration.GetSection("CustomersModule").Bind(customerOptions);
            services.AddSingleton(customerOptions);

            services.Configure<CustomerOptions>(Configuration.GetSection("CustomersModule"));
        }

~~~



## REST API


| Akcja  | Opis                  |
|--------|-----------------------|
| GET    | Pobierz               |
| POST   | Utwórz                |
| PUT    | Podmień               |
| DELETE | Usuń                  |
| PATCH  | Zmień częściowo       |
| HEAD   | Czy zasób istnieje    |

## Opcje serializacji json

Plik Startup.cs

~~~ csharp

public void ConfigureServices(IServiceCollection services)
{
  services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // Wyłączenie generowania wartości null w jsonie
        options.SerializerSettings.Converters.Add(new StringEnumConverter(camelCaseText: true));  // Serializacja enum jako tekst
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Zapobieganie cyklicznej serializacji

    })
}
~~~

### Włączenie obsługi XML

- Instalacja
~~~ bash
dotnet add package AddXmlSerializerFormatters
~~~


Plik Startup.cs

~~~ csharp
 public void ConfigureServices(IServiceCollection services)
 {
     services
         .AddMvc(options => options.RespectBrowserAcceptHeader = true)
         .AddXmlSerializerFormatters();
 }
~~~

### Przekazywanie formatu poprzez adres URL



~~~ csharp

// GET api/customers/10
// GET api/customers/10.json
// GET api/customers/10.xml

[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [FormatFilter]
    [HttpGet("{id:int}.{format?}")]
    public IActionResult GetById(int id)
    {
        if (!customerRepository.IsExists(id))
            return NotFound();

        var customer = customerRepository.Get(id);

        return Ok(customer);
    }
}
~~~


## Autentyfikacja


### Basic
Headers 

| Key   | Value  |
|---|---|
| Authorization | Basic {Base64(login:password)}  |


### Utworzenie uchwytu

~~~ csharp

 public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUsersService usersService;

        public BasicAuthenticationHandler(
            IUsersService usersService,
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {

            this.usersService = usersService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing authorization header");
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");

            var username = credentials[0];
            var password = credentials[1];

            User user = usersService.Authenticate(username, password);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            IIdentity identity = new ClaimsIdentity(Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            // IIdentity identity = new GenericIdentity(user.Login);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
    }
~~~

#### Rejestracja
Startup.cs

~~~ csharp
 public void ConfigureServices(IServiceCollection services)
{

    services.AddAuthentication("BasicAuthorization")
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthorization", null);
  }
  
 public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {


      app.UseAuthentication();
      app.UseMvc();
    }

~~~

### Token
Headers 

| Key   | Value  |
|---|---|
| Authorization | Bearer {token}  |


### JWT

https://github.com/sulmar/dotnet-core-jwt


## OWIN

Startup.cs

~~~ csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseOwin(pipeline => pipeline(next => OwinHandler));
}
            
public Task OwinHandler(IDictionary<string, object> environment)
{
     string responseText = "Hello World via OWIN";
     byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

    // OWIN Environment Keys: http://owin.org/spec/spec/owin-1.0.0.html

    var requestMethod = (string) environment["owin.RequestMethod"];
    var requestScheme = (string) environment["owin.RequestScheme"];
    var requestHeaders = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];
    var requestQueryString = (string) environment["owin.RequestQueryString"];         

    var responseStream = (Stream)environment["owin.ResponseBody"];

    var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];

    responseHeaders["Content-Length"] = new string[] { responseBytes.Length.ToString(CultureInfo.InvariantCulture) };
    responseHeaders["Content-Type"] = new string[] { "text/plain" };

    return responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
}

~~~



## Middleware


### Run
~~~ csharp
 public void Configuration(IAppBuilder app)
{
  app.Run(async context => await context.Response.WriteAsync("Hello World"));
}
~~~


### Uworzenie metody własnej warstwy pośredniej

~~~ csharp
app.Use(async (context, next) =>
{
    Trace.WriteLine(String.Format("request: {0} - {1}", context.Request.Method, context.Request.Path));

    await next.Invoke();

    Trace.WriteLine(String.Format("response: {0}", context.Response.StatusCode));

});
~~~




### Uworzenie klasy własnej warstwy pośredniej

Na przykładzie przekazywania formatu poprzez URL, np. http://localhost:5000/api/values?format=application/xml

RequestAcceptMiddleware.cs

~~~ csharp
 public class RequestAcceptMiddleware
    {
        private readonly RequestDelegate next;

        public RequestAcceptMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var formatQuery = context.Request.Query["format"];

            if (!string.IsNullOrWhiteSpace(formatQuery))
            {
                context.Request.Headers.Remove("Accept");
                context.Request.Headers.Append("Accept", new string[] { formatQuery });
            }

            // Call the next delegate/middleware in the pipeline
            await next(context);

        }
    }
    
  
  ~~~
  
   Startup.cs
 
  
~~~ csharp 
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
     app.UseMiddleware<RequestAcceptMiddleware>();
}
~~~
  
### Zastosowanie metody rozszerzającej
  
RequestAcceptMiddlewareExtensions.cs
  
~~~ csharp 
  public static class RequestAcceptMiddlewareExtensions
  {
      public static IApplicationBuilder UseRequestAccept(
          this IApplicationBuilder builder)
      {
          return builder.UseMiddleware<RequestAcceptMiddleware>();
      }
  }
~~~
  
Startup.cs

~~~ csharp 
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseRequestAccept();
}
~~~

### Mapowanie tras

~~~ csharp 
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{

    app.Map("/dashboard", HandleMapTest1);

    app.Map("/sensors", node =>
      {
          node.Map("/temp", TempDelegate);
          node.Map("/humidity", HumidityDelegate);
          node.Map(string.Empty, SensorsDelegate);
      });
}

private void HumidityDelegate(IAppBuilder app)
{
    app.Run(async context => await context.Response.WriteAsync("1024 hPa"));
}

private void TempDelegate(IAppBuilder app)
{
    app.Run(async context => await context.Response.WriteAsync("Temp 23C"));
}
~~~

### Mapowanie warunkowe
~~~ csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{       
    app.MapWhen(context => context.Request.Headers.Get("Host").StartsWith("localhost"), LocalHostDelegate);
 }
 
private void LocalHostDelegate(IAppBuilder app)
{
    app.Run(async context => await context.Response.WriteAsync("localhost"));
}

~~~

### Mapowanie akcji

~~~ csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {       

            var rb = new RouteBuilder(app);

            rb.Routes.Add(new Route(new MyRouter(), "fib/{number:int}",
                app.ApplicationServices.GetService<IInlineConstraintResolver>()));

            rb.MapGet("", request => request.Response.WriteAsync("Hello World"));

            rb.MapGet("sensors", request => request.Response.WriteAsync("Sensors"));

            rb.MapGet("sensors/{id:int}", request => request.Response.WriteAsync($"Sensor id {request.GetRouteValue("id")}"));

            rb.MapPost("post", request => request.Response.WriteAsync("Created"));

            app.UseRouter(rb.Build());
~~~

## Signal-R

### Utworzenie koncetratora (huba)

CustomersHub.cs

~~~ csharp

public class CustomersHub : Hub
{
     public override Task OnConnectedAsync()
     {
         return base.OnConnectedAsync();
     }

     public Task CustomerAdded(Customer customer)
     {
         return this.Clients.Others.SendAsync("Added", customer);
     } 
     
      public Task Ping(string message="Pong")
      {
           return this.Clients.Caller.SendAsync(message);
      }
     
}
~~~

### Rejestracja koncentratora

~~~ csharp

public class Startup
{
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseSignalR(routes => routes.MapHub<CustomersHub>("/hubs/customers"));

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Run(async (context) =>
        {
            await context.Response.WriteAsync("Hello World!");
        });
    }
~~~

### Utworzenie nadawcy


~~~ csharp

static async Task Main(string[] args)
{
    const string url = "http://localhost:5000/hubs/customers";

    HubConnection connection = new HubConnectionBuilder()
        .WithUrl(url)
        .Build();

    connection.Closed += ex => Task.Run(() => System.Console.WriteLine($"ERROR {ex.Message}"));
    await connection.StartAsync();

    Customer customer = new Customer
    {
        FirstName = "Marcin",
        LastName = "Sulecki"
    };

    while (true)
    {
        await connection.SendAsync("CustomerAdded", customer);
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
~~~


### Utworzenie odbiorcy

~~~ csharp

static async Task Main(string[] args)
{
    const string url = "http://localhost:5000/hubs/customers";

    // dotnet add package Microsoft.AspNetCore.SignalR.Client

    HubConnection connection = new HubConnectionBuilder()
        .WithUrl(url)
        .Build();

    connection.Closed +=  ex => Task.Run(()=>System.Console.WriteLine($"ERROR {ex.Message}")); 

    await connection.StartAsync();            

    connection.On<Customer>("Added", 
        customer => Console.WriteLine($"Added customer {customer.FirstName}"));
}
~~~

### Wstrzykiwanie huba

CustomersController.cs

~~~ csharp

 public class CustomersController : ControllerBase
 {
    private readonly IHubContext<CustomersHub> hubContext;
   
    public CustomersController(IHubContext<CustomersHub> hubContext)
     {
         this.hubContext = hubContext;
     }
     
    [HttpPost]
     public async Task<IActionResult> Post( Customer customer)
     {
         customersService.Add(customer);

         await hubContext.Clients.All.SendAsync("Added", customer);

         return CreatedAtRoute(new { Id = customer.Id }, customer);
     }
 }

~~~

### Autentykacja

Program.cs

~~~ csharp
static async Task Main(string[] args)
{
    const string url = "http://localhost:5000/hubs/customers";

    var username = "your-username";
    var password = "your-password";

    var credentialBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
    var credentials = Convert.ToBase64String(credentialBytes);

    string parameter = $"Basic {credentials}";

    HubConnection connection = new HubConnectionBuilder()
        .WithUrl(url, options => options.Headers.Add("Authorization", parameter))
        .Build();

    await connection.StartAsync();        
    await connection.SendAsync("CustomerAdded", customer);
}

~~~

### Utworzenie silnie typowanego huba

CustomersHub.cs

~~~ csharp

public interface ICustomersHub
{
    Task Added(Customer customer);
}

public class CustomersHub : Hub<ICustomersHub>
{
     public Task CustomerAdded(Customer customer)
     {
         return this.Clients.Others.Added(customer);
     } 
}
~~~

### Wstrzykiwanie silnie typowanego huba

CustomersController.cs

~~~ csharp

 public class CustomersController : ControllerBase
 {
    private readonly IHubContext<CustomersHub, ICustomersHub> hubContext;
   
    public CustomersController(IHubContext<CustomersHub, ICustomersHub> hubContext)
     {
         this.hubContext = hubContext;
     }
     
    [HttpPost]
     public async Task<IActionResult> Post( Customer customer)
     {
         customersService.Add(customer);

         await hubContext.Clients.All.Added(customer);

         return CreatedAtRoute(new { Id = customer.Id }, customer);
     }
 }

~~~


### Grupy


~~~ csharp

public async Task AddToGroup(string groupName)
{
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
}

public async Task RemoveFromGroup(string groupName)
{
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
}

~~~



## Testy jednostkowe


### NUnit


Utworzenie projektu

~~~ bash
dotnet new nunit
~~~

Przykładowa klasa

~~~ csharp
public class Calculator
{
    public int Add(int x, int y) => x + y;
}
~~~


#### Test
~~~ csharp

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
         var calculator = new Calculator();

        var result = calculator.Add(2, 2);

        Assert.AreEqual(4, result);
    }
}
~~~


### xUnit

Utworzenie projektu

~~~ bash
dotnet new xunit
~~~

Przykładowa klasa

~~~ csharp
public class Calculator
{
    public int Add(int x, int y) => x + y;
}
~~~

#### Fakt

~~~ csharp
[Fact]
    public void Test1()
    {
        var calculator = new Calculator();

        int result = calculator.Add(2, 2);

        Assert.Equal(4, result);

    }
~~~

#### Teoria - inlinedata

~~~ csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(-4, -6, -10)]
[InlineData(-2, 2, 0)]
[InlineData(int.MinValue, -1, int.MaxValue)]
public void CanAddTheory(int value1, int value2, int expected)
{
    var calculator = new Calculator();
    var result = calculator.Add(value1, value2);
    Assert.Equal(expected, result);
}
~~~


#### Teoria - classdata

~~~ csharp
public class CalculatorTestData : TheoryData<int, int, int>
{
    public CalculatorTestData()
    {
        Add(1, 2, 3);
        Add(-4, -6, -10);
        Add(-2, 2, 0);
        Add(int.MinValue, -1, int.MaxValue);
        
    }
}

[Theory]
[ClassData(typeof(CalculatorTestData))]
public void CanAdd(int value1, int value2, int expected)
{
    var calculator = new Calculator();
    var result = calculator.Add(value1, value2);
    Assert.Equal(expected, result);
}
~~~


#### Teoria - memberdata


~~~ csharp

public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { -4, -6, -10 },
            new object[] { -2, 2, 0 },
            new object[] { int.MinValue, -1, int.MaxValue },
        };

[Theory]
  [MemberData(nameof(Data))]
  public void CanAddTheoryMemberDataProperty(int value1, int value2, int expected)
  {
      var calculator = new Calculator();

      var result = calculator.Add(value1, value2);

      Assert.Equal(expected, result);
  }
~~~

### FluentAssertions


## Kontrola kondycji

### Rejestrowanie kondycji

#### Kondycja SQL Server

~~~ bash
dotnet add package AspNetCore.HealthChecks.SqlServer 
~~~

Startup.cs

~~~ csharp

public void ConfigureServices(IServiceCollection services)
{
 services.AddHealthChecksUI()
    .AddSqlServer(Configuration.GetConnectionStrings("MyConnection");
}

~~~


#### Kondycja DbContext

~~~ bash
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
~~~

Startup.cs

~~~ csharp

public void ConfigureServices(IServiceCollection services)
{
  services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
            
    services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

}

~~~


### Utworzenie własnej kontroli kondycji


RandomHealthCheck.cs

~~~ csharp

public class RandomHealthCheck  : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (DateTime.UtcNow.Minute % 2 == 0)
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(description: "failed"));
        }
    }
~~~

Startup.cs

~~~ csharp

public void ConfigureServices(IServiceCollection services)
{
 services.AddHealthChecks()
             .AddCheck<RandomHealthCheck>("random");
}

~~~

### Dashboard

Instalacja

~~~ bash
dotnet add package AspNetCore.HealthChecks.UI
~~~
          

Startup.cs

~~~ csharp

public void ConfigureServices(IServiceCollection services)
{
   services.AddHealthChecksUI();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseHealthChecks("/health",  new HealthCheckOptions()
      {
          Predicate = _ => true,
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });
}
~~~

appsettings.json

~~~ json

 "HealthChecks-UI": {
    "HealthChecks": [
      {
        "Name": "Http and UI on single project",
        "Uri": "http://localhost:5000/health"
      }
    ],
    "Webhooks": [],
    "EvaluationTimeOnSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
  
~~~

Wskazówka: Przejdź na http://localhost:5000/healthchecks-ui aby zobaczyc panel

## Generowanie dokumentacji
W formacie Swagger/OpenApi

### Instalacja

~~~ bash
dotnet add TodoApi.csproj package Swashbuckle.AspNetCore
~~~

### Konfiguracja

Plik Startup.cs

~~~ csharp
public void ConfigureServices(IServiceCollection services)
{
 services
      .AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "My Api", Version = "1.0" }));         
} 
~~~

~~~ csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
   app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
 }           
~~~

## Docker


- Uruchomienie pierwszego kontenera
~~~ bash
docker run ubuntu /bin/echo 'Hello world'
~~~

- Uruchomienie w trybie interaktywnym
~~~ bash
docker run -i -t --rm ubuntu /bin/bash
~~~

### Przydatne komendy
- ``` docker images ``` - lista wszystkich obrazów na twojej maszynie
- ``` docker pull <image> ``` - pobranie obrazu
- ``` docker run <image> ``` - uruchomienie obrazu (pobiera jeśli nie ma)
- ``` docker ps ``` - lista wszystkich uruchomionych kontenerów na twojej maszynie
- ``` docker ps -a``` - lista wszystkich przyłączonych ale nie uruchomionych kontenerów
- ``` docker start <containter_name> ``` - uruchomienie kontenera wg nazwy
- ``` docker stop <containter_name> ``` - zatrzymanie kontenera wg nazwy

### Konteneryzacja aplikacji .NET Core

* Utwórz plik Dockerfile

~~~
FROM microsoft/dotnet:2.0-sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/Hello.dll"]
~~~


## ngrok

- Uruchomienie

``` bash
ngrok http 5000
```

- Interfejs webowy

```
http://127.0.0.1:4040
```

- API

```
http://127.0.0.1:4040/api
```
