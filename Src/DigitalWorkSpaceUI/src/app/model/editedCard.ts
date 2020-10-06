 export interface IEditedCard
{
    id:number;
    version : number;
    imageContent : Blob;
    title :string;
    description : string;
    favicon:string;
}