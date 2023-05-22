# Getting tokens from Azure AD

We already have an application registered in Azure Ad and we're going to use that to authenticate.

1. Open up the login page, specifying the application we're using with the client ID parameter and return URL.

   PowerShell on Windows

   ```pwsh
   start 'https://login.microsoftonline.com/aaaaws.com/oauth2/v2.0/authorize?client_id=416c20b6-5f42-4941-9591-40f160fcfce7&response_type=code&redirect_uri=http://localhost/demo&response_mode=query&scope=openid%20offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fuser.read&state=12345'
   ```

   Terminal on MacOS

   ```bash
   open 'https://login.microsoftonline.com/aaaaws.com/oauth2/v2.0/authorize?client_id=416c20b6-5f42-4941-9591-40f160fcfce7&response_type=code&redirect_uri=http://localhost/demo&response_mode=query&scope=openid%20offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fuser.read&state=12345'
   ```

   Or just copy the URL into a browser.

2. After logging in, Azure AD will redirect the browser to the address we specified, `http://localhost/demo`. We shouldn't have anything running on our machine so the request will fail. This is expected. In the URL, find the `code` parameter and copy that.

   Include it in this command line using `curl`:

   PowerShell:

   ```pwsh
   curl --location 'https://login.microsoftonline.com/aaaaws.com/oauth2/v2.0/token' `
   --header 'Content-Type: application/x-www-form-urlencoded' `
   --data-urlencode 'grant_type=authorization_code' `
   --data-urlencode 'client_id=416c20b6-5f42-4941-9591-40f160fcfce7' `
   --data-urlencode 'scope=openid https://graph.microsoft.com/user.read' `
   --data-urlencode 'redirect_uri=http://localhost/demo' `
   --data-urlencode 'client_secret=bKD8Q~p6MDa9koCWPfmmTAowDoKRZ~3qwHLBhbm0' `
   --data-urlencode 'code=<INSERT CODE HERE>'
   ```
   
   Bash:
   
   ```bash
   curl --location 'https://login.microsoftonline.com/aaaaws.com/   oauth2/v2.0/token' \
   --header 'Content-Type: application/x-www-form-urlencoded' \
   --data-urlencode 'grant_type=authorization_code' \
   --data-urlencode 'client_id=416c20b6-5f42-4941-9591-40f160fcfce7' \
   --data-urlencode 'scope=openid' \
   --data-urlencode 'redirect_uri=http://localhost/demo' \
   --data-urlencode 'client_secret=bKD8Q~p6MDa9koCWPfmmTAowDoKRZ~3qwHLBhbm0' \
   --data-urlencode 'code=<INSERT CODE HERE>'
   ```

3. Copy the returned `id_token` and `access_token` *values* into https://jwt.ms and examine them.

4. Test your token with the Microsoft Graph API

   Call the user profile endpoint. Replace the placeholder text with the *access token* from the previous response. 

   PowerShell: 

   ```pwsh
   curl --location 'https://graph.microsoft.com/v1.0/me' `
   --header 'Authorization: Bearer <the-access-token-copied-from-last-response>'
   ```

   Bash:

   ```bash
   curl --location 'https://graph.microsoft.com/v1.0/me' \
   --header 'Authorization: Bearer <the-access-token-copied-from-last-response>'
   ```

5. Try calling the `https://graph.microsoft.com/oidc/userinfo` endpoint with the same bearer token as above. This is an endpoint defined in the OpenID Connect standard.
