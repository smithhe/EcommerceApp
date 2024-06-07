export class Category {
    id: number;
    name: string;
    summary: string;

    constructor(
        id: number,
        name: string,
        summary: string
    ) {
        this.id = id;
        this.name = name;
        this.summary = summary;
    }
}
