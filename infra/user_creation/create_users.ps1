#  Create users in Azure AD from a CSV file
Import-Csv -Path "users.csv" | 
ForEach-Object { 
    az ad user create `
        --display-name "$($_.Firstname) $($_.Lastname)" `
        --user-principal-name "u$("{0:D2}" -f [int]$_.Number)@aaaaws.com" `
        --password "NDCw2023" 
}

# Get the user IDs of all users
$users = az ad user list --query "[?starts_with(userPrincipalName, 'u')].id" --output tsv

# Assign the "Application administrator" role to each user
$application_administrator_id = "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3"
foreach ($user in $users) {
    az rest --method POST `
        --uri 'https://graph.microsoft.com/beta/roleManagement/directory/roleAssignments' `
        --body "{'principalId': '$user', 'roleDefinitionId': '$application_administrator_id', 'directoryScopeId': '/'}"

}   