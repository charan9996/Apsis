export const authConfig = {
	authority: 'https://login.microsoftonline.com/5ced40db-2fc3-4ef1-b1b3-48b4e94668f5',
	clientID: '2f93b081-7abe-490c-ace5-eeeb111a42b0',
	//redirectUri: 'https://apsis-dev-ui.azurewebsites.net',
	// redirectUri: 'https://apsis-dev.azurewebsites.net',
	redirectUri: 'http://localhost:4200',
	scopes: ['People.Read'] // People.Read.All - Requires an Administrator to grant this permission
};
