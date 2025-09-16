export interface IRoute {
    path: string;
    component: any;
    auth?: boolean;
    routes?: Array<IRoute>;
}
