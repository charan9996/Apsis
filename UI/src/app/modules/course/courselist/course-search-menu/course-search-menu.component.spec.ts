import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseSearchMenuComponent } from './course-search-menu.component';

describe('CourseSearchMenuComponent', () => {
  let component: CourseSearchMenuComponent;
  let fixture: ComponentFixture<CourseSearchMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseSearchMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseSearchMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
