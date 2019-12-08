import { CustomUiComponentsModule } from './custom-ui-components.module';

describe('CustomUiComponentsModule', () => {
  let customUiComponentsModule: CustomUiComponentsModule;

  beforeEach(() => {
    customUiComponentsModule = new CustomUiComponentsModule();
  });

  it('should create an instance', () => {
    expect(customUiComponentsModule).toBeTruthy();
  });
});
