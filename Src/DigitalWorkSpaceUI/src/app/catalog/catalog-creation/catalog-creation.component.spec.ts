import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CatalogCreationComponent } from './catalog-creation.component';

describe('CatalogCreationComponent', () => {
  let component: CatalogCreationComponent;
  let fixture: ComponentFixture<CatalogCreationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CatalogCreationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CatalogCreationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
