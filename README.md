# Explanation
Our company has a fleet of aircraft that is tracked by an online service. The service does not have a public API for customers to call on their own.
My company needed accurate time information for automated maintenance document software and logbook tracking. This is a work-around where the times are manually scraped
off the website and stored into a DB. This is set as an hourly job via Hangfire. As an extra, it is served via a very simple web API. 
# Setup
#### Software
A valid version of chromedriver on PATH that is valid with your current installed version of Chrome is necessary as the scraping is done via Chromedriver.
#### Database
Migrations are committed to the repository. Sqlite databases will be generated on launch. 
#### Login Configuration
Login configuration is stored in user secrets and injected via the IOptions<TOptions> interface.
```
cd FspScraper.WebAPI
dotnet user-secrets set "Login:FspUser" "<your_username>"
dotnet user-secrets set "Email:FspPass" "<your_key>"
```