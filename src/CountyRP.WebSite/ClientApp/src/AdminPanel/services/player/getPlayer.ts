import { Player } from 'AdminPanel/types'; 


export async function getPlayer(id: string = '') {
  const apiUrl = process.env.REACT_APP_API_URL;
  const url = `${apiUrl}api/Admin/Player/${id}`;

  const response = await fetch(url);

  if (!response.ok)
    throw new Error(`${response.statusText}`);

  const json: Player = await response.json();

  if (!json?.id || !json?.login)
    throw new Error(`Response is invalid: ${json.toString()}`);

  return json;
}