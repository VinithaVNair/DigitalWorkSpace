export class ICard
{
    id:number;
    version : number;
    shortUrl : string;
    favicon :string;
    imageContent : Blob;
    title :string;
    description : string;
    expiry : Date;
    isLinked : boolean;
}