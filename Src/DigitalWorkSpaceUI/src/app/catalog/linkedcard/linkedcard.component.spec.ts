import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkedcardComponent } from './linkedcard.component';

describe('LinkedcardComponent', () => {
  let component: LinkedcardComponent;
  let fixture: ComponentFixture<LinkedcardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkedcardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkedcardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
