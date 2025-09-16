import { RenderComponent } from '../../enums/render-component.enum';
import { TestCase } from '../models/test-case';

export type TestRenderDict = {
    [key in RenderComponent]?: TestCase[];
};
